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
using Dribbly.Model.Notifications;
using Newtonsoft.Json;

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

        public async Task CancelJoinRequest(long groupId)
        {
            var accountId = _securityUtility.GetAccountId();
            var request = await _context.JoinGroupRequests
                .SingleOrDefaultAsync(r => r.RequestorId == accountId && r.GroupId == groupId);
            if (request != null)
            {
                _context.JoinGroupRequests.Remove(request);
                await _context.SaveChangesAsync();
            }
        }

        public async Task JoinGroupAsync(long groupId)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    var accountId = _securityUtility.GetAccountId().Value;
                    var account = await _context.Accounts.SingleOrDefaultAsync(a => a.Id == accountId);
                    if (account == null)
                    {
                        throw new DribblyObjectNotFoundException($"Couldn't find account with ID {accountId}",
                            friendlyMessageKey: "Account details could not be found.");
                    }
                    var group = await _context.Groups
                        .Include(g => g.JoinRequests)
                        .SingleOrDefaultAsync(g => g.Id == groupId);
                    if (group == null)
                    {
                        throw new DribblyObjectNotFoundException($"Group with ID {groupId} not found.",
                            "The group's info could not be found. It may have been deleted.");
                    }

                    if (group.JoinRequests.Any(g => g.RequestorId == accountId))
                    {
                        throw new DribblyInvalidOperationException($"Account ID {accountId} tried to request duplicate join group request. Group ID: {groupId}",
                            friendlyMessageKey: "You already currently have a pending request to join this group");
                    }

                    var request = new JoinGroupRequest
                    {
                        RequestorId = accountId,
                        GroupId = groupId,
                        DateAdded = DateTime.UtcNow
                    };
                    _context.JoinGroupRequests.Add(request);
                    await _context.SaveChangesAsync();

                    await _notificationsRepo.TryAddAsync(new NotificationModel
                    {
                        ForUserId = group.AddedById,
                        DateAdded = DateTime.UtcNow,
                        Type = NotificationTypeEnum.JoinGroupRequest,
                        AdditionalInfo = JsonConvert.SerializeObject(new
                        {
                            requestId = request.Id,
                            requestorName = account.Name,
                            groupName = group.Name,
                            groupId = group.Id
                        })
                    });
                    await _indexedEntitysRepository.Update(_context, group);
                    tx.Commit();
                }
                catch (Exception)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public async Task RemoveMemberAsync(long groupId, long accountId)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {                    
                    var member = await _context.GroupMembers
                        .Include(m => m.Group)
                        .SingleOrDefaultAsync(m => m.GroupId == groupId && m.AccountId == accountId);
                    if(member != null)
                    {
                        var accountid = _securityUtility.GetAccountId();
                        if (member.Group.AddedById != accountid)
                        {
                            throw new DribblyForbiddenException($"Non-admin tried to remove group member. Group ID: {member.GroupId}, Account ID: {accountId}",
                                friendlyMessage: "You do not have permission to remove members from this group");
                        }
                        _context.GroupMembers.Remove(member);
                        await _context.SaveChangesAsync();
                        tx.Commit();
                    }
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
                .Include(g => g.JoinRequests.Select(r => r.Requestor.ProfilePhoto))
                .SingleOrDefaultAsync(g => g.Id == groupId);
            return new GroupViewerModel(model, accountId);
        }

        public async Task<GroupUserRelationship> GetGroupUserRelationshipAsync(long groupId)
        {
            var accountId = _securityUtility.GetAccountId();
            var group = await _context.Groups
                .Include(g => g.JoinRequests)
                .Include(g => g.Members.Select(m => m.Account.ProfilePhoto))
                .SingleOrDefaultAsync(g => g.Id == groupId);
            return new GroupUserRelationship(group, accountId);
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

        public async Task ProcessJoinRequestAsync(long requestId, bool isApproved)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    var request = await _context.JoinGroupRequests.SingleOrDefaultAsync(r => r.Id == requestId);
                    var accountId = _securityUtility.GetAccountId().Value;
                    if (request == null)
                    {
                        throw new DribblyObjectNotFoundException($"Unable to find request with ID {requestId}.",
                            friendlyMessageKey: "The join request does not exist");
                    }

                    var group = await _context.Groups.SingleAsync(g => g.Id == request.GroupId);
                    var isAdmin = group.AddedById == accountId;
                    if (!isAdmin)
                    {
                        throw new DribblyForbiddenException("Non-admin of group attempted to process a join request.",
                            friendlyMessageKey: "You do not have the permission to process this request.");
                    }

                    if (isApproved)
                    {
                        var member = new GroupMemberModel
                        {
                            AccountId = request.RequestorId,
                            GroupId = request.GroupId,
                            DateJoined = DateTime.UtcNow
                        };

                        _context.GroupMembers.Add(member);

                        //TODO: log user activity
                        await _notificationsRepo.TryAddAsync(new NotificationModel
                        {
                            ForUserId = accountId,
                            DateAdded = DateTime.UtcNow,
                            Type = NotificationTypeEnum.JoinGroupRequestApproved,
                            AdditionalInfo = JsonConvert.SerializeObject(new
                            {
                                groupName = group.Name,
                                groupId = group.Id
                            })
                        });
                    }

                    _context.JoinGroupRequests.Remove(request);
                    await _context.SaveChangesAsync();
                    tx.Commit();
                }
                catch (Exception e)
                {
                    // TODO: log error
                    tx.Rollback();
                    throw;
                }
            }
        }
    }
    public interface IGroupsService
    {
        Task CancelJoinRequest(long groupId);
        Task<GroupModel> CreateGroupAsync(AddEditGroupInputModel input);
        Task<GroupViewerModel> GetGroupViewerData(long groupId);
        Task JoinGroupAsync(long groupId);
        Task ProcessJoinRequestAsync(long requestId, bool isApproved);
        Task RemoveMemberAsync(long groupId, long accountId);
        Task<MultimediaModel> SetLogoAsync(long groupId);
        Task<GroupModel> UpdateGroupAsync(AddEditGroupInputModel input);
    }
}