using Dribbly.Core.Utilities;
using Dribbly.Email.Services;
using Dribbly.Model;
using Dribbly.Model.Account;
using Dribbly.Model.Comments;
using Dribbly.Model.Entities;
using Dribbly.Service.Repositories;
using Dribbly.Service.Services.Shared;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public class CommentsService : BaseEntityService<CommentModel>, ICommentsService
    {
        IAuthContext _context;
        ISecurityUtility _securityUtility;
        IAccountRepository _accountRepo;
        private readonly IIndexedEntitysRepository _indexedEntitysRepository;
        private readonly INotificationsRepository _notificationsRepo;

        ICommonService _commonService { get; }

        public CommentsService(IAuthContext context,
            IEmailService emailSender,
            ISecurityUtility securityUtility) : base(context.Comments, context)
        {
            _context = context;
            _securityUtility = securityUtility;
            _accountRepo = new AccountRepository(context, new AuthRepository(emailSender, context));
            _commonService = new CommonService(context, _securityUtility);
            _indexedEntitysRepository = new IndexedEntitysRepository(context);
            _notificationsRepo = new NotificationsRepository(context);
        }

        public async Task<IEnumerable<CommentModel>> GetComments(GetCommentsInputModel input)
        {
            var comments = await _context.Comments
                .Include(c => c.AddedBy.User)
                .Include(c => c.AddedBy.ProfilePhoto)
                .OrderByDescending(c => c.DateAdded)
                .Where(c => c.CommentedOnType == input.CommentedOnType
                && (!input.CommentedOnId.HasValue || c.CommentedOnId == input.CommentedOnId)
                && (!input.AfterDate.HasValue || c.DateAdded < input.AfterDate))
                .OrderByDescending(c => c.DateAdded)
                .Take(input.PageSize)
                .ToListAsync();

            return comments;
        }

        public async Task<CommentModel> AddComment(AddCommentInputModel input)
        {
            var user = _securityUtility.GetAccountId();
            var comment = new CommentModel
            {
                DateAdded = DateTime.UtcNow,
                AddedById = user.Value,
                Message = input.Message,
                CommentedOnType = input.CommentedOnType,
                CommentedOnId = input.CommentedOnId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return await _context.Comments
                .Include(c => c.AddedBy.User)
                .Include(c => c.AddedBy.ProfilePhoto)
                .SingleAsync(c => c.Id == comment.Id);
        }
    }

    public interface ICommentsService
    {
        Task<CommentModel> AddComment(AddCommentInputModel input);
        Task<IEnumerable<CommentModel>> GetComments(GetCommentsInputModel input);
    }
}