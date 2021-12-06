using Dribbly.Core.Exceptions;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Account;
using Dribbly.Model.Bookings;
using Dribbly.Model.Notifications;
using Dribbly.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;

namespace Dribbly.Service.Services
{
    public class BookingsService : BaseEntityService<BookingModel>, IBookingsService
    {
        IAuthContext _context;
        HttpContextBase _httpContext;
        ISecurityUtility _securityUtility;
        IFileService _fileService;
        IAccountRepository _accountRepo;
        INotificationsRepository _notificationsRepo;
        ICourtsRepository _courtsRepo;

        public BookingsService(IAuthContext context,
            HttpContextBase httpContext,
            ISecurityUtility securityUtility,
            IAccountRepository accountRepo,
            IFileService fileService,
            INotificationsRepository notificationsRepo,
            ICourtsRepository courtsRepo) : base(context.Bookings)
        {
            _context = context;
            _httpContext = httpContext;
            _securityUtility = securityUtility;
            _accountRepo = accountRepo;
            _fileService = fileService;
            _notificationsRepo = notificationsRepo;
            _courtsRepo = courtsRepo;
        }

        public IEnumerable<BookingModel> GetAll()
        {
            return All();
        }

        public async Task<BookingModel> GetBooking(long id)
        {
            var booking = await _dbSet.Include(g => g.Court).SingleOrDefaultAsync(g => g.Id == id);
            if (booking != null)
            {
                booking.BookedBy = await _accountRepo.GetAccountBasicInfo(booking.BookedById);
                var account = await _accountRepo.GetAccountByIdentityId(booking.BookedById);
                if (account != null)
                {
                    booking.BookedByChoice = new AccountsChoicesItemModel(account);
                }
                else
                {
                    throw new DribblyObjectNotFoundException
                        ($"Unable to find account with {nameof(account.IdentityUserId)} {booking.BookedById}");
                }
            }
            return booking;
        }

        public async Task<BookingModel> AddBookingAsync(BookingModel booking)
        {
            var currentUserId = _securityUtility.GetUserId();
            booking.AddedBy = currentUserId.Value;
            booking.Status = Enums.BookingStatusEnum.Approved;
            Add(booking);
            _context.SaveChanges();
            NotificationTypeEnum Type = booking.BookedById == currentUserId ?
                NotificationTypeEnum.NewGameForOwner :
                NotificationTypeEnum.NewGameForBooker;
            await _notificationsRepo.TryAddAsync(new NewBookingNotificationModel
            {
                BookingId = booking.Id,
                BookedById = booking.BookedById,
                ForUserId = Type == NotificationTypeEnum.NewGameForBooker ? booking.BookedById :
                (await _courtsRepo.GetOwnerId(booking.CourtId)),
                DateAdded = DateTime.UtcNow,
                Type = Type
            });

            return booking;
        }

        public async Task UpdateBookingAsync(BookingModel booking)
        {
            Update(booking);
            var currentUserId = _securityUtility.GetUserId();
            NotificationTypeEnum Type = booking.BookedById == currentUserId ?
                NotificationTypeEnum.NewGameForOwner :
                NotificationTypeEnum.NewGameForBooker;
            await _notificationsRepo.TryAddAsync(new NewBookingNotificationModel
            {
                BookingId = booking.Id,
                BookedById = booking.BookedById,
                ForUserId = Type == NotificationTypeEnum.NewGameForBooker ? booking.BookedById :
                (await _courtsRepo.GetOwnerId(booking.CourtId)),
                DateAdded = DateTime.UtcNow,
                Type = Type
            });
            _context.SaveChanges();
        }
    }
}