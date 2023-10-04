using Dribbly.Chat.Services;
using Dribbly.Core.Enums;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.DTO.Groups;
using Dribbly.Model.Entities.Groups;
using Dribbly.Service.Repositories;
using Dribbly.Service.Services.Shared;
using System;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;
using Dribbly.Core.Exceptions;
using Dribbly.Core.Models;
using System.Web;

namespace Dribbly.Service.Services
{
    public class GroupsService : BaseEntityService<GroupModel>, IGroupsService
    {
        private readonly IAuthContext _context;
        private readonly ISecurityUtility _securityUtility;
        private readonly IFileService _fileService;
        private readonly IAccountRepository _accountRepo;
        private readonly INotificationsRepository _notificationsRepo;
        private readonly ICourtsRepository _courtsRepo;
        private readonly ICommonService _commonService;
        private readonly IIndexedEntitysRepository _indexedEntitysRepository;
        private readonly IDribblyChatService _dribblyChatService;

        public GroupsService(IAuthContext context,
            ISecurityUtility securityUtility,
            IAccountRepository accountRepo,
            IFileService fileService,
            INotificationsRepository notificationsRepo,
            ICourtsRepository courtsRepo,
            IIndexedEntitysRepository indexedEntitysRepository) : base(context.Groups, context)
        {
            _context = context;
            _securityUtility = securityUtility;
            _accountRepo = accountRepo;
            _fileService = fileService;
            _notificationsRepo = notificationsRepo;
            _courtsRepo = courtsRepo;
            _commonService = new CommonService(context, securityUtility);
            _indexedEntitysRepository = indexedEntitysRepository;
            _dribblyChatService = new DribblyChatService(context);
        }

        public async Task<GroupModel> CreateGroupAsync(AddEditGroupInputModel input)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    var accountId = _securityUtility.GetAccountId().Value;
                    var group = new GroupModel
                    {
                        Name = input.Name,
                        AddedById = accountId,
                        DateAdded = DateTime.UtcNow
                    };
                    group.EntityStatus = EntityStatusEnum.Active;
                    Add(group);
                    _context.SaveChanges();
                    await _indexedEntitysRepository.Add(_context, group);
                    tx.Commit();
                    return group;
                }
                catch (Exception)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public async Task<GroupModel> UpdateGroupAsync(AddEditGroupInputModel input)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    var accountId = _securityUtility.GetAccountId().Value;
                    var group = await _context.Groups.SingleOrDefaultAsync(g => g.Id == input.Id);
                    if (group == null)
                    {
                        throw new DribblyObjectNotFoundException($"Group with ID {input.Id} not found.",
                            "The group's info could not be found. It may have been deleted.");
                    }
                    group.Name = input.Name;
                    await _context.SaveChangesAsync();
                    await _indexedEntitysRepository.Update(_context, group);
                    tx.Commit();
                    return group;
                }
                catch (Exception)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public async Task<GroupViewerModel> GetGroupViewerData(long groupId)
        {
            var accountId = _securityUtility.GetAccountId();
            var model = await _context.Groups
                .Include(g => g.Logo)
                .Include(g => g.Members.Select(m => m.Account.ProfilePhoto))
                .SingleOrDefaultAsync(g => g.Id == groupId);
            return new GroupViewerModel(model, accountId);
        }

        public async Task<MultimediaModel> SetLogoAsync(long groupId)
        {
            GroupModel group = GetById(groupId);
            var accountId = _securityUtility.GetAccountId();

            if ((accountId != group.AddedById)) //TODO: update after we implement multiple group admins
            {
                throw new DribblyForbiddenException("Authorization failed when attempting to update account primary photo.",
                    friendlyMessage: "Only a group admin can update the group logo.");
            }

            HttpFileCollection files = HttpContext.Current.Request.Files;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    MultimediaModel photo = await AddPhoto(group, files[0]);
                    group.LogoId = photo.Id;
                    Update(group);
                    await _context.SaveChangesAsync();
                    await _indexedEntitysRepository.SetIconUrl(_context, group, photo.Url);
                    // TODO: log user activity
                    transaction.Commit();
                    return photo;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        private async Task<MultimediaModel> AddPhoto(GroupModel group, HttpPostedFile file)
        {
            var accountId = _securityUtility.GetAccountId().Value;
            string uploadPath = _fileService.Upload(file, accountId + "/group_photos/");

            MultimediaModel photo = new MultimediaModel
            {
                Url = uploadPath,
                UploadedById = accountId,
                DateAdded = DateTime.UtcNow
            };
            _context.Multimedia.Add(photo);
            await _context.SaveChangesAsync();

            return photo;
        }
    }
    public interface IGroupsService
    {
        Task<GroupModel> CreateGroupAsync(AddEditGroupInputModel input);
        Task<GroupViewerModel> GetGroupViewerData(long groupId);
        Task<MultimediaModel> SetLogoAsync(long groupId);
        Task<GroupModel> UpdateGroupAsync(AddEditGroupInputModel input);
    }
}