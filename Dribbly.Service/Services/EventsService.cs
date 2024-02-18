using Dribbly.Chat.Services;
using Dribbly.Core.Enums;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.DTO.Events;
using Dribbly.Model.Entities.Events;
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
    public class EventsService : BaseEntityService<EventModel>, IEventsService
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

        public EventsService(IAuthContext context,
            ISecurityUtility securityUtility,
            IAccountRepository accountRepo,
            IFileService fileService,
            INotificationsRepository notificationsRepo,
            ICourtsRepository courtsRepo,
            IIndexedEntitysRepository indexedEntitysRepository) : base(context.Events, context)
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

        public async Task<EventModel> CreateEventAsync(AddEditEventInputModel input)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    var accountId = _securityUtility.GetAccountId().Value;
                    var evt = new EventModel
                    {
                        Title = input.Title,
                        Description = input.Description,
                        AddedById = accountId,
                        DateAdded = DateTime.UtcNow,
                        StartDate = input.StartDate,
                        EndDate = input.EndDate,
                        CourtId = input.CourtId
                    };
                    evt.EntityStatus = EntityStatusEnum.Active;
                    Add(evt);
                    _context.SaveChanges();
                    await _indexedEntitysRepository.Add(_context, evt);
                    tx.Commit();
                    return evt;
                }
                catch (Exception e)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public async Task CancelJoinRequest(long eventId)
        {
            var accountId = _securityUtility.GetAccountId();
            var request = await _context.EventAttendees
                .SingleOrDefaultAsync(r => r.AccountId == accountId && r.EventId == eventId && !r.IsApproved);
            if (request != null)
            {
                _context.EventAttendees.Remove(request);
                await _context.SaveChangesAsync();
            }
        }

        public async Task LeaveEventAsync(long eventId)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    var accountId = _securityUtility.GetAccountId().Value;
                    var membership = await _context.EventAttendees
                        .SingleOrDefaultAsync(m => m.EventId == eventId && m.AccountId == accountId);

                    if (membership != null)
                    {
                        _context.EventAttendees.Remove(membership);
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

        public async Task JoinEventAsync(long eventId)
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
                    var evt = await _context.Events
                        .Include(g => g.Attendees)
                        .SingleOrDefaultAsync(g => g.Id == eventId);
                    if (evt == null)
                    {
                        throw new DribblyObjectNotFoundException($"Event with ID {eventId} not found.",
                            "The event's info could not be found. It may have been deleted.");
                    }

                    if (evt.Attendees.Any(g => g.AccountId == accountId))
                    {
                        throw new DribblyInvalidOperationException($"Account ID {accountId} tried to request duplicate join evt request. Event ID: {eventId}",
                            friendlyMessageKey: "You already currently have a pending request to join this event");
                    }

                    var request = new EventAttendeeModel
                    {
                        AccountId = accountId,
                        EventId = eventId,
                        DateJoined = DateTime.UtcNow,
                        IsApproved = false
                    };
                    _context.EventAttendees.Add(request);
                    await _context.SaveChangesAsync();

                    await _notificationsRepo.TryAddAsync(new NotificationModel
                    {
                        ForUserId = evt.AddedById,
                        DateAdded = DateTime.UtcNow,
                        Type = NotificationTypeEnum.JoinEventRequest,
                        AdditionalInfo = JsonConvert.SerializeObject(new
                        {
                            requestId = request.Id,
                            requestorName = account.Name,
                            eventName = evt.Name,
                            eventId = evt.Id
                        })
                    });
                    await _indexedEntitysRepository.Update(_context, evt);
                    tx.Commit();
                }
                catch (Exception)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public async Task RemoveMemberAsync(long eventId, long accountId)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    var member = await _context.EventAttendees
                        .Include(m => m.Event)
                        .SingleOrDefaultAsync(m => m.EventId == eventId && m.AccountId == accountId);
                    if (member != null)
                    {
                        var accountid = _securityUtility.GetAccountId();
                        if (member.Event.AddedById != accountid)
                        {
                            throw new DribblyForbiddenException($"Non-admin tried to remove evt member. Event ID: {member.EventId}, Account ID: {accountId}",
                                friendlyMessage: "You do not have permission to remove members from this event");
                        }
                        _context.EventAttendees.Remove(member);
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

        public async Task<EventModel> UpdateEventAsync(AddEditEventInputModel input)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    var accountId = _securityUtility.GetAccountId().Value;
                    var evt = await _context.Events.SingleOrDefaultAsync(g => g.Id == input.Id);
                    if (evt == null)
                    {
                        throw new DribblyObjectNotFoundException($"Event with ID {input.Id} not found.",
                            "The event's info could not be found. It may have been deleted.");
                    }
                    evt.Title = input.Title;
                    evt.Description = input.Description;
                    evt.StartDate = input.StartDate;
                    evt.EndDate = input.EndDate;
                    evt.CourtId = input.CourtId;
                    await _context.SaveChangesAsync();
                    await _indexedEntitysRepository.Update(_context, evt);
                    tx.Commit();
                    return evt;
                }
                catch (Exception)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public async Task<EventViewerModel> GetEventViewerData(long eventId)
        {
            var accountId = _securityUtility.GetAccountId();
            var model = await _context.Events
                .Include(g => g.Logo)
                .Include(g=>g.AddedBy.ProfilePhoto)
                .Include(g => g.Attendees.Select(m => m.Account.ProfilePhoto))
                .Include(g => g.Court)
                .SingleOrDefaultAsync(g => g.Id == eventId);

            if (model == null)
            {
                return null;
            }

            return new EventViewerModel(model, accountId);
        }

        public async Task<EventUserRelationship> GetEventUserRelationshipAsync(long eventId)
        {
            var accountId = _securityUtility.GetAccountId();
            var evt = await _context.Events
                .Include(g => g.Attendees)
                .Include(g => g.Attendees.Select(m => m.Account.ProfilePhoto))
                .SingleOrDefaultAsync(g => g.Id == eventId);
            return new EventUserRelationship(evt, accountId);
        }

        public async Task<MultimediaModel> SetLogoAsync(long eventId)
        {
            EventModel evt = GetById(eventId);
            var accountId = _securityUtility.GetAccountId();

            if ((accountId != evt.AddedById)) //TODO: update after we implement multiple evt admins
            {
                throw new DribblyForbiddenException("Authorization failed when attempting to update account primary photo.",
                    friendlyMessage: "Only a evt admin can update the evt logo.");
            }

            HttpFileCollection files = HttpContext.Current.Request.Files;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    MultimediaModel photo = await AddPhoto(evt, files[0]);
                    evt.LogoId = photo.Id;
                    Update(evt);
                    await _context.SaveChangesAsync();
                    await _indexedEntitysRepository.SetIconUrl(_context, evt, photo.Url);
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

        private async Task<MultimediaModel> AddPhoto(EventModel evt, HttpPostedFile file)
        {
            var accountId = _securityUtility.GetAccountId().Value;
            string uploadPath = _fileService.Upload(file, accountId + "/event_photos/");

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
                    var request = await _context.EventAttendees.SingleOrDefaultAsync(r => r.Id == requestId && !r.IsApproved);
                    var accountId = _securityUtility.GetAccountId().Value;
                    if (request == null)
                    {
                        throw new DribblyObjectNotFoundException($"Unable to find request with ID {requestId}.",
                            friendlyMessageKey: "The join request does not exist");
                    }

                    var evt = await _context.Events.SingleAsync(g => g.Id == request.EventId);
                    var isAdmin = evt.AddedById == accountId;
                    if (!isAdmin)
                    {
                        throw new DribblyForbiddenException("Non-admin of evt attempted to process a join request.",
                            friendlyMessageKey: "You do not have the permission to process this request.");
                    }

                    if (isApproved)
                    {
                        request.IsApproved = true;

                        //TODO: log user activity
                        await _notificationsRepo.TryAddAsync(new NotificationModel
                        {
                            ForUserId = request.AccountId,
                            DateAdded = DateTime.UtcNow,
                            Type = NotificationTypeEnum.JoinEventRequestApproved,
                            AdditionalInfo = JsonConvert.SerializeObject(new
                            {
                                eventName = evt.Name,
                                eventId = evt.Id
                            })
                        });
                    }

                    _context.EventAttendees.Remove(request);
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
    public interface IEventsService
    {
        Task CancelJoinRequest(long eventId);
        Task<EventModel> CreateEventAsync(AddEditEventInputModel input);
        Task<EventViewerModel> GetEventViewerData(long eventId);
        Task JoinEventAsync(long eventId);
        Task LeaveEventAsync(long eventId);
        Task ProcessJoinRequestAsync(long requestId, bool isApproved);
        Task RemoveMemberAsync(long eventId, long accountId);
        Task<MultimediaModel> SetLogoAsync(long eventId);
        Task<EventModel> UpdateEventAsync(AddEditEventInputModel input);
    }
}