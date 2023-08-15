using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Posts;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using Dribbly.Service.Repositories;
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
                Type = input.Type
            };
            post.AddedById = _securityUtility.GetAccountId().Value;
            Add(post);
            await _context.SaveChangesAsync();
            await _indexedEntitysRepository.Add(_context, post);
            await _commonService.AddUserPostActivity(UserActivityTypeEnum.AddPost, post.Id);
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
