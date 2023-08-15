using Dribbly.Authentication.Services;
using Dribbly.Core.Enums.Permissions;
using Dribbly.Core.Exceptions;
using Dribbly.Core.Utilities;
using Dribbly.Email.Services;
using Dribbly.Model;
using Dribbly.Model.Posts;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using Dribbly.Service.Repositories;
using Dribbly.Service.Services.Shared;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Dribbly.Service.Services
{
    public class PostsService : BaseEntityService<PostModel>, IPostsService
    {
        IAuthContext _context;
        ISecurityUtility _securityUtility;
        IAccountRepository _accountRepo;
        private readonly IIndexedEntitysRepository _indexedEntitysRepository;
        private readonly ISharedPostsService _sharedPostsService;

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
            long? postOnIdLong;
            if (input.PostedOnType == EntityTypeEnum.Account)
            {
                postOnIdLong = await _accountRepo.GetIdentityUserAccountId(input.PostedOnId);
            }
            else
            {
                postOnIdLong = input.PostedOnId;
            }
            var posts = await _context.Posts
                .Include(p=>p.AddedBy.User)
                .Include(p=>p.AddedBy.ProfilePhoto)
                .Where(p => p.PostedOnType == input.PostedOnType && p.PostedOnId == postOnIdLong &&
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

        public async Task<PostModel> AddPost(AddEditPostInputModel input)
        {
            return await _sharedPostsService.AddPostAsync(input);
        }

        public async Task<PostModel> UpdatePost(AddEditPostInputModel input)
        {
            PostModel post = _context.Posts.SingleOrDefault(p => p.Id == input.Id);
            if (_securityUtility.IsCurrentAccount(post.AddedById))
            {
                post.Content = input.Content;
                await _context.SaveChangesAsync();
                await _indexedEntitysRepository.Update(_context, post);
                await _commonService.AddUserPostActivity(UserActivityTypeEnum.UpdatePost, post.Id);
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

        Task<bool> DeletePost(long Id);

    }
}