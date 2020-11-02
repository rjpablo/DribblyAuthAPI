using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Account;
using Dribbly.Model.Games;
using Dribbly.Model.Notifications;
using Dribbly.Model.Posts;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using Dribbly.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Dribbly.Service.Services
{
    public class PostsService : BaseEntityService<PostModel>, IPostsService
    {
        IAuthContext _context;
        HttpContextBase _httpContext;
        ISecurityUtility _securityUtility;
        IFileService _fileService;
        IAccountRepository _accountRepo;
        INotificationsRepository _notificationsRepo;
        ICourtsRepository _courtsRepo;

        public PostsService(IAuthContext context,
            HttpContextBase httpContext,
            ISecurityUtility securityUtility,
            IAccountRepository accountRepo,
            IFileService fileService,
            INotificationsRepository notificationsRepo,
            ICourtsRepository courtsRepo) : base(context.Posts)
        {
            _context = context;
            _httpContext = httpContext;
            _securityUtility = securityUtility;
            _accountRepo = accountRepo;
            _fileService = fileService;
            _notificationsRepo = notificationsRepo;
            _courtsRepo = courtsRepo;
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
                postOnIdLong = long.Parse(input.PostedOnId);
            }
            var posts = await _context.Posts
                .Where(p => p.PostedOnType == input.PostedOnType && p.PostedOnId == postOnIdLong &&
                p.Status == EntityStatusEnum.Active && (!input.CeilingPostId.HasValue || p.Id < input.CeilingPostId))
                .Take(input.GetCount).OrderByDescending(p => p.Id).ToListAsync();

            foreach (PostModel post in posts)
            {
                if (post.AddedByType == EntityTypeEnum.Account)
                {
                    post.AddedBy = await GetAddedBy(post);
                }
            }

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
            long postOnIdLong = input.PostedOnType == EntityTypeEnum.Account ?
                (await _accountRepo.GetIdentityUserAccountId(input.PostedOnId)).Value :
                postOnIdLong = long.Parse(input.PostedOnId);

            PostModel post = new PostModel
            {
                AddedByType = input.AddedByType,
                PostedOnType = input.PostedOnType,
                PostedOnId = postOnIdLong,
                Content = input.Content,
                Status = EntityStatusEnum.Active
            };
            var currentUserId = _securityUtility.GetUserId();
            post.AddedById = currentUserId;
            Add(post);
            await _context.SaveChangesAsync();
            post.AddedBy = await GetAddedBy(post);
            return post;
        }

        public async Task<PostModel> UpdatePost(AddEditPostInputModel input)
        {
            PostModel post = _context.Posts.SingleOrDefault(p => p.Id == input.Id);
            post.Content = input.Content;
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<bool> DeletePost(long Id)
        {
            var currentUserId = _securityUtility.GetUserId();
            PostModel post = _context.Posts.SingleOrDefault(p => p.Id == Id);
            if (post != null) {

                if(post.Status == EntityStatusEnum.Deleted)
                {
                    throw new InvalidOperationException("app.ThisPostHasAlreadyBeenDeleted");
                }

                if (currentUserId == post.AddedById)
                {
                    post.Status = EntityStatusEnum.Deleted;
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            else
            {
                throw new ObjectNotFoundException("app.ThisPostHasAlreadyBeenDeleted");
            }
            return false;
        }
    }
}