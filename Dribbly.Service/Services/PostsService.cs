using Dribbly.Authentication.Services;
using Dribbly.Core.Enums;
using Dribbly.Core.Enums.Permissions;
using Dribbly.Core.Exceptions;
using Dribbly.Core.Utilities;
using Dribbly.Email.Services;
using Dribbly.Model;
using Dribbly.Model.Entities;
using Dribbly.Model.Entities.Posts;
using Dribbly.Model.Enums;
using Dribbly.Model.Notifications;
using Dribbly.Model.Posts;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using Dribbly.Service.Repositories;
using Dribbly.Service.Services.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public class PostsService : BaseEntityService<PostModel>, IPostsService
    {
        IAuthContext _context;
        ISecurityUtility _securityUtility;
        IAccountRepository _accountRepo;
        private readonly IIndexedEntitysRepository _indexedEntitysRepository;
        private readonly ISharedPostsService _sharedPostsService;
        private readonly INotificationsRepository _notificationsRepo;

        ICommonService _commonService { get; }

        public PostsService(IAuthContext context,
            IEmailService emailSender,
            ISecurityUtility securityUtility) : base(context.Posts, context)
        {
            _context = context;
            _securityUtility = securityUtility;
            _accountRepo = new AccountRepository(context, new AuthRepository(emailSender, context));
            _commonService = new CommonService(context, _securityUtility);
            _indexedEntitysRepository = new IndexedEntitysRepository(context);
            _sharedPostsService = new SharedPostsService(context, securityUtility, _accountRepo, _commonService, new IndexedEntitysRepository(context)); ;
            _notificationsRepo = new NotificationsRepository(context);
        }

        /// <summary>
        /// Get posts for a specified entity
        /// </summary>
        /// <param name="postOnType"></param>
        /// <param name="postOnId"></param>
        /// <param name="getCount">The maximum number of posts to return</param>
        /// <param name="ceilingPostId">Return only posts with IDs lower than this number. If null, do not filter posts by Id.</param>
        /// <returns></returns>
        public async Task<IEnumerable<PostModel>> GetPosts(GetPostsInputModel input)
        {
            var posts = await _context.Posts
                .Include(p => p.AddedBy.User)
                .Include(p => p.AddedBy.ProfilePhoto)
                .Include(p => p.Files.Select(f => f.File))
                .OrderByDescending(p => p.DateAdded)
                .Include(p => p.Reactions.Select(r => r.Reactor.ProfilePhoto))
                .Where(p => (input.PostedOnType == EntityTypeEnum.All || (p.PostedOnType == input.PostedOnType &&
                (!input.PostedOnId.HasValue || (p.PostedOnId == input.PostedOnId)))) &&
                p.EntityStatus == EntityStatusEnum.Active && (!input.CeilingPostId.HasValue || p.Id < input.CeilingPostId))
                .Take(input.GetCount).OrderByDescending(p => p.Id).ToListAsync();

            return posts;
        }

        private async Task<EntityBasicInfoModel> GetAddedBy(PostModel post)
        {
            if (post.AddedByType == EntityTypeEnum.Account)
            {
                var account = await _accountRepo.GetAccountById(post.AddedById);
                return new EntityBasicInfoModel(account);
            }

            return null;
        }

        public async Task AddReaction(PostReactionInput input)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    var accountId = _securityUtility.GetAccountId().Value;
                    var account = await _context.Accounts.SingleOrDefaultAsync(a => a.Id == accountId);
                    var post = await _context.Posts.SingleOrDefaultAsync(p => p.Id == input.PostId);
                    var reaction = new PostReaction
                    {
                        DateAdded = DateTime.UtcNow,
                        PostId = input.PostId,
                        Type = input.ReactionType,
                        ReactorId = accountId
                    };

                    _context.PostReactions.Add(reaction);
                    await _context.SaveChangesAsync();

                    if (accountId != post.AddedById)
                    {
                        await _notificationsRepo.TryAddAsync(new NotificationModel
                        {
                            ForUserId = post.AddedById,
                            DateAdded = DateTime.UtcNow,
                            Type = NotificationTypeEnum.PostReceivedReaction,
                            AdditionalInfo = JsonConvert.SerializeObject(new
                            {
                                reactionType = input.ReactionType,
                                reactorName = account.Name
                            })
                        });
                    }
                    tx.Commit();
                }
                catch (Exception)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public async Task RemoveReaction(PostReactionInput input)
        {
            var accountId = _securityUtility.GetAccountId().Value;
            var reaction = await _context.PostReactions
                .SingleOrDefaultAsync(r => r.PostId == input.PostId && r.Type == input.ReactionType
                && r.ReactorId == accountId);
            if (reaction != null)
            {
                _context.PostReactions.Remove(reaction);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PostModel> AddPost(AddEditPostInputModel input)
        {
            return await _sharedPostsService.AddPostAsync(input);
        }

        public async Task<PostModel> UpdatePost(AddEditPostInputModel input)
        {
            PostModel post = _context.Posts
                .Include(p => p.Files)
                .SingleOrDefault(p => p.Id == input.Id);
            if (_securityUtility.IsCurrentAccount(post.AddedById))
            {
                var origFileIds = post.Files.Select(f => f.FileId).ToList();
                var newFileIds = input.FileIds.Where(id => !origFileIds.Contains(id)).ToList();
                var fileIdsToDelete = origFileIds.Where(id => !input.FileIds.Contains(id)).ToList();
                //remove deleted files
                foreach (var id in fileIdsToDelete)
                {
                    var file = post.Files.Single(f => f.FileId == id);
                    post.Files.Remove(file);
                    _context.PostFiles.Remove(file);
                }
                // assign Order
                int order = 1;
                foreach (var id in input.FileIds)
                {
                    PostFile file = post.Files.SingleOrDefault(f => f.FileId == id);
                    if (file == null) // new added file
                    {
                        file = new PostFile
                        {
                            FileId = id,
                            PostId = post.Id
                        };
                        post.Files.Add(file);
                    }
                    file.Order = order++;
                }
                post.Content = input.Content;
                post.EmbedCode = input.EmbedCode;
                await _context.SaveChangesAsync();
                await _indexedEntitysRepository.Update(_context, post);
                await _commonService.AddUserPostActivity(UserActivityTypeEnum.UpdatePost, post.Id);
                post.Files = await _context.PostFiles
                    .Include(f => f.File)
                    .Where(f => f.PostId == post.Id)
                    .ToListAsync();
                return post;
            }
            else
            {
                throw new DribblyForbiddenException(string.Format("Authorization failed when attempting to update post with ID {0}", post.Id),
                    friendlyMessageKey: "app.Error_EditPostNotAllowed");
            }
        }

        public async Task<bool> DeletePost(long Id)
        {
            PostModel post = _context.Posts.SingleOrDefault(p => p.Id == Id);
            if (post != null)
            {
                if (post.EntityStatus == EntityStatusEnum.Deleted)
                {
                    throw new DribblyInvalidOperationException(string.Format("Tried to delete a posts that has already been deleted. Post ID: {0}", post.Id),
                        friendlyMessageKey: "app.ThisPostHasAlreadyBeenDeleted");
                }

                if (_securityUtility.IsCurrentAccount(post.AddedById) || AuthenticationService.HasPermission(PostPermission.DeleteNotOwned))
                {
                    post.EntityStatus = EntityStatusEnum.Deleted;
                    await _context.SaveChangesAsync();
                    await _indexedEntitysRepository.Update(_context, post);
                    await _commonService.AddUserPostActivity(UserActivityTypeEnum.DeletePost, Id);
                    return true;
                }
                else
                {
                    throw new DribblyForbiddenException(string.Format("Authorization failed when attempting to delete post with ID {0}", post.Id),
                        friendlyMessageKey: "app.Error_NotAllowedToDeletePost");
                }
            }
            else
            {
                throw new DribblyInvalidOperationException(string.Format("Tried to delete a posts that does not exist. Post ID: {0}", post.Id),
                           friendlyMessageKey: "app.ThisPostHasAlreadyBeenDeleted");
            }
        }
    }

    public interface IPostsService
    {
        Task<IEnumerable<PostModel>> GetPosts(GetPostsInputModel input);

        Task<PostModel> AddPost(AddEditPostInputModel input);

        Task<PostModel> UpdatePost(AddEditPostInputModel input);
        Task AddReaction(PostReactionInput input);
        Task RemoveReaction(PostReactionInput input);

        Task<bool> DeletePost(long Id);

    }
}