using Dribbly.Core.Enums;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Entities;
using Dribbly.Model.Posts;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using Dribbly.Service.Repositories;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Services.Shared
{
    public class SharedPostsService : BaseEntityService<PostModel>, ISharedPostsService
    {
        private readonly IAuthContext _context;
        private readonly ISecurityUtility _securityUtility;
        private readonly IAccountRepository _accountRepo;
        private readonly IIndexedEntitysRepository _indexedEntitysRepository;

        ICommonService _commonService { get; }

        public SharedPostsService(IAuthContext context,
            ISecurityUtility securityUtility,
            IAccountRepository accountRepo,
            ICommonService commonService,
            IIndexedEntitysRepository indexedEntitysRepository) : base(context.Posts, context)
        {
            _context = context;
            _securityUtility = securityUtility;
            _accountRepo = accountRepo;
            _commonService = commonService;
            _indexedEntitysRepository = indexedEntitysRepository;
        }

        public async Task<PostModel> AddPostAsync(AddEditPostInputModel input)
        {
            PostModel post = new PostModel
            {
                AddedByType = input.AddedByType,
                PostedOnType = input.PostedOnType,
                PostedOnId = input.PostedOnId,
                Content = input.Content,
                EntityStatus = EntityStatusEnum.Active,
                AdditionalData = input.AdditionalData,
                Type = input.Type,
                EmbedCode = input.EmbedCode
            };
            post.AddedById = _securityUtility.GetAccountId().Value;
            Add(post);
            int order = 1;
            foreach (long fileId in input.FileIds)
            {
                _context.PostFiles.Add(new PostFile
                {
                    Post = post,
                    FileId = fileId,
                    Order = order++
                });
            }
            await _context.SaveChangesAsync();
            await _indexedEntitysRepository.Add(_context, post);
            await _commonService.AddUserPostActivity(UserActivityTypeEnum.AddPost, post.Id);
            post.Files = await _context.PostFiles
                .Include(f => f.File)
                .Where(f => f.PostId == post.Id)
                .ToListAsync();

            post.AddedBy = await _context.Players
                .Include(a => a.ProfilePhoto)
                .Include(a => a.User)
                .SingleAsync(a => a.Id == post.AddedById);
            return post;
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
    }

    public interface ISharedPostsService
    {
        Task<PostModel> AddPostAsync(AddEditPostInputModel input);
    }
}
