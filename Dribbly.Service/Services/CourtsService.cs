using Dribbly.Authentication.Services;
using Dribbly.Core.Enums.Permissions;
using Dribbly.Core.Exceptions;
using Dribbly.Core.Models;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Bookings;
using Dribbly.Model.Courts;
using Dribbly.Model.Games;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using Dribbly.Service.Repositories;
using Dribbly.Service.Services.Shared;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;

namespace Dribbly.Service.Services
{
    public class CourtsService : BaseEntityService<CourtModel>, ICourtsService
    {
        IAuthContext _context;
        ISecurityUtility _securityUtility;
        IFileService _fileService;
        ICourtsRepository _courtsRepo;
        IAccountRepository _accountRepo;
        private readonly ICommonService _commonService;
        private readonly IIndexedEntitysRepository _indexedEntitysRepository;

        public CourtsService(IAuthContext context,
            ISecurityUtility securityUtility,
            IFileService fileService,
            ICourtsRepository courtsRepo,
            IAccountRepository accountRepo,
            ICommonService commonService,
            IIndexedEntitysRepository indexedEntitysRepository) : base(context.Courts, context)
        {
            _context = context;
            _securityUtility = securityUtility;
            _fileService = fileService;
            _courtsRepo = courtsRepo;
            _accountRepo = accountRepo;
            _commonService = commonService;
            _indexedEntitysRepository = indexedEntitysRepository;
        }

        #region Generic Court methods

        public async Task<IEnumerable<CourtDetailsViewModel>> GetAllActiveAsync()
        {
            CourtModel[] courts = await _context.Courts.Where(c=>c.EntityStatus == EntityStatusEnum.Active)
                .Include(p => p.PrimaryPhoto).ToArrayAsync();
            List<CourtDetailsViewModel> viewModels = new List<CourtDetailsViewModel>();

            foreach (var court in courts)
            {
                CourtDetailsViewModel vm = new CourtDetailsViewModel(court);
                vm.FollowerCount = await getFollowerCountAsync(court.Id);
                await PopulateOwner(vm);
                viewModels.Add(vm);
            }

            return viewModels;
        }

        public async Task<CourtDetailsViewModel> GetCourtAsync(long id)
        {
            CourtModel court = _context.Courts.Include(p => p.PrimaryPhoto).Include(p => p.Contact).SingleOrDefault(p => p.Id == id);
            var result = new CourtDetailsViewModel(court);
            if(result.EntityStatus == EntityStatusEnum.Deleted)
            {
                // Do not populate other properties if court has been deleted to save some resources
                return result;
            }
            long? currentAccountId = _securityUtility.GetAccountId();
            result.IsFollowed = _context.CourtFollowings.Any(f => currentAccountId.HasValue && f.CourtId == id && f.FollowedById == currentAccountId);

            Task populateOwnerTask = PopulateOwner(result);
            Task<long> followerCountTask = getFollowerCountAsync(court.Id);
            Task<Model.Bookings.BookingModel> mostRecentBookingTask = GetCurrentUserMostRecentBooking(court.Id);
            await Task.WhenAll(populateOwnerTask, followerCountTask, mostRecentBookingTask);

            result.FollowerCount = followerCountTask.Result;
            result.ReviewCount = await _context.CourtReivews.Where(r => r.CourtId == id).CountAsync();
            result.CanReview = mostRecentBookingTask.Result == null ? false : true;
            return result;
        }

        private async Task<Model.Bookings.BookingModel> GetCurrentUserMostRecentBooking(long courtId)
        {
            using (AuthContext context = new AuthContext())
            {
                if (_securityUtility.IsAuthenticated())
                {
                    long? accountId = _securityUtility.GetAccountId();
                    BookingModel mostRecentBooking = await context.Bookings
                        .Where(e => accountId.HasValue && e.CourtId == courtId && e.BookedById == accountId &&
                        e.Start < DateTime.UtcNow && e.End < DateTime.UtcNow && e.Status == Enums.BookingStatusEnum.Approved &&
                        !e.HasReviewed).OrderByDescending(e => e.End).FirstOrDefaultAsync();
                    return mostRecentBooking;
                }
                return null;
            }
        }

        public async Task<CourtReviewModalModel> GetCodeReviewModalAsync(long courtId)
        {
            Model.Bookings.BookingModel mostRecentBooking = await GetCurrentUserMostRecentBooking(courtId);
            if (mostRecentBooking == null)
            {
                return null;
            }

            return new CourtReviewModalModel
            {
                Booking = mostRecentBooking,
                Court = await GetByIdAsync(courtId)
            };
        }

        private async Task<long> getFollowerCountAsync(long courtId)
        {
            using (AuthContext context = new AuthContext())
            {
                return await context.CourtFollowings.Where(f => f.CourtId == courtId).CountAsync();
            }
        }

        private async Task PopulateOwner(CourtDetailsViewModel court)
        {
            court.Owner = await _accountRepo.GetAccountBasicInfo(court.OwnerId);
        }

        public async Task<IEnumerable<CourtModel>> FindActiveCourtsAsync(CourtSearchInputModel input)
        {
            return await _courtsRepo.FindActiveCourtsAsync(input);
        }

        public async Task<FollowResultModel> FollowCourtAsync(long courtId, bool isFollowing)
        {
            long? currentAccountId = _securityUtility.GetAccountId();
            FollowResultModel result = new FollowResultModel();
            CourtFollowingModel courtFollowing = await _context.CourtFollowings
                .SingleOrDefaultAsync(f => currentAccountId.HasValue && f.CourtId == courtId && f.FollowedById == currentAccountId);
            result.isAlreadyFollowing = courtFollowing != null;
            result.IsNotCurrentlyFollowing = !result.isAlreadyFollowing;

            if (isFollowing)
            {
                if (!result.isAlreadyFollowing)
                {
                    _context.CourtFollowings.Add(new CourtFollowingModel
                    {
                        CourtId = courtId,
                        FollowedById = currentAccountId.Value
                    });
                    await _context.SaveChangesAsync();
                    await _commonService.AddUserCourtActivity(UserActivityTypeEnum.FollowCourt, courtId);
                    result.isSuccessful = true;
                }

            }
            else
            {
                // if unfollowing court
                if (result.isAlreadyFollowing)
                {
                    _context.CourtFollowings.Remove(courtFollowing);
                    await _context.SaveChangesAsync();
                    result.isSuccessful = true;
                }

            }

            result.newFollowerCount = await getFollowerCountAsync(courtId); ;
            return result;
        }

        public async Task<long> RegisterAsync(CourtModel court)
        {
            court.OwnerId = _securityUtility.GetAccountId().Value;
            court.EntityStatus = EntityStatusEnum.Active;
            Add(court);
            _context.SaveChanges();
            _context.IndexedEntities.Add(new IndexedEntityModel(court));
            _context.SaveChanges();
            await _commonService.AddUserCourtActivity(UserActivityTypeEnum.AddCourt, court.Id);
            return court.Id;
        }

        public async Task UpdateCourtPhoto(long courtId)
        {
            HttpFileCollection files = HttpContext.Current.Request.Files;
            string uploadPath = _fileService.Upload(files[0], "court/");
            CourtModel court = GetById(courtId);

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    PhotoModel photo = new PhotoModel
                    {
                        Url = uploadPath,
                        UploadedById = _securityUtility.GetAccountId().Value,
                        DateAdded = DateTime.Now
                    };
                    _context.Photos.Add(photo);
                    await _context.SaveChangesAsync();
                    _context.CourtPhotos.Add(new CourtPhotoModel
                    {
                        CourtId = courtId,
                        PhotoId = photo.Id
                    });
                    court.PrimaryPhotoId = photo.Id;
                    UpdateCourt(court);
                    await _indexedEntitysRepository.SetIconUrl(_context, court, photo.Url);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    await _commonService.AddCourtPhotosActivity(UserActivityTypeEnum.SetCourtPrimaryPhoto, court.Id, photo);
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public async Task UpdateCourtAsync(CourtModel court)
        {
            if (court.OwnerId == _securityUtility.GetAccountId().Value || AuthenticationService.HasPermission(CourtPermission.UpdateNotOwned))
            {
                Update(court);
                await _context.SaveChangesAsync();
                await _indexedEntitysRepository.Update(_context, court);
                await _commonService.AddUserCourtActivity(UserActivityTypeEnum.UpdatCourt, court.Id);
            }
            else
            {
                throw new DribblyForbiddenException("Authorization failed when attempting to update court details.");
            }
        }

        public async Task UpdateCourtPropertiesAsync(GenericEntityUpdateInputModel input)
        {
            CourtModel court = await _dbSet.SingleOrDefaultAsync(c => c.Id == input.Id);
            if (court.OwnerId == _securityUtility.GetAccountId().Value || AuthenticationService.HasPermission(CourtPermission.UpdateNotOwned))
            {
                foreach(KeyValuePair<string, object> item in input.Properties)
                {
                    court[item.Key] = item.Value;
                }
                await _context.SaveChangesAsync();
                await _indexedEntitysRepository.Update(_context, court);
                await _commonService.AddUserCourtActivity(UserActivityTypeEnum.UpdatCourt, court.Id);
            }
            else
            {
                throw new DribblyForbiddenException("Authorization failed when attempting to update court details.");
            }
        }

        private void UpdateCourt(CourtModel court)
        {
            if (court.OwnerId == _securityUtility.GetAccountId().Value || AuthenticationService.HasPermission(CourtPermission.UpdateNotOwned))
            {
                Update(court);
            }
            else
            {
                throw new DribblyForbiddenException("Authorization failed when attempting to update court details.");
            }
        }

        public async Task DeleteCourtAsync(long courtId)
        {
            var court = await _dbSet.FirstOrDefaultAsync(c => c.Id == courtId);

            if(court == null)
            {
                throw new DribblyObjectNotFoundException($"Attempted to delete non-existent court. Court ID: {courtId}");
            }

            if (court.OwnerId == _securityUtility.GetAccountId().Value || AuthenticationService.HasPermission(CourtPermission.DeleteNotOwned))
            {
                court.EntityStatus = EntityStatusEnum.Deleted;
                await _context.SaveChangesAsync();
                await _indexedEntitysRepository.Update(_context, court);
            }
            else
            {
                throw new DribblyForbiddenException("Authorization failed when attempting to delete court.", friendlyMessageKey: "app.Error_DeleteCourtForbidden");
            }
        }

        #endregion

        #region Reviews

        public async Task<IEnumerable<CourtReviewModel>> GetReviewsAsync(long courtId)
        {
            var reviews = await _context.CourtReivews.Where(r => r.CourtId == courtId)
                .OrderByDescending(r => r.DateAdded).ToListAsync();
            foreach (CourtReviewModel review in reviews)
            {
                review.ReviewedBy = await _accountRepo.GetAccountBasicInfo(review.ReviewedById);
            }

            return reviews;
        }

        public async Task SubmitReviewAsync(CourtReviewModel review)
        {
            review.ReviewedById = _securityUtility.GetAccountId().Value;
            CourtModel court = GetById(review.CourtId);
            if (court.OwnerId == review.ReviewedById)
            {
                throw new DribblyInvalidOperationException
                    (string.Format("Tried to rate own court. Court ID: {0}, User ID: {1}", court.Id, court.OwnerId),
                    friendlyMessageKey: "app.Error_CantRateOwnCourt");
            }
            Model.Bookings.BookingModel reviewBooking = _context.Bookings.SingleOrDefault(e => e.Id == review.BookingId);
            if (reviewBooking == null)
            {
                throw new DribblyInvalidOperationException
                    (string.Format("Tried to rate non-existing court. Court ID: {0}", court.Id, court.OwnerId),
                    friendlyMessageKey: "app.Error_CourtNotFound");
            }
            reviewBooking.HasReviewed = true;
            review.DateAdded = DateTime.UtcNow;
            _context.CourtReivews.AddOrUpdate(review);
            await _context.SaveChangesAsync();
            court.Rating = await _context.CourtReivews.Where(r => r.CourtId == review.CourtId).AverageAsync(r => r.Rating);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Court Games

        public async Task<IEnumerable<GameModel>> GetCourtGamesAsync(long courtId)
        {
            var games = await _context.Games.Where(g => g.CourtId == courtId).ToListAsync();
            return games;
        }

        #endregion

        #region Court Bookings

        public IEnumerable<BookingModel> GetCourtBookings(long courtId)
        {
            var bookings = _context.Bookings.Where(g => g.CourtId == courtId).ToList();
            return bookings;
        }

        #endregion

        #region Court Photos

        public async Task<IEnumerable<PhotoModel>> AddPhotosAsync(long courtId)
        {
            HttpFileCollection files = HttpContext.Current.Request.Files;
            List<PhotoModel> photos = new List<PhotoModel>();
            for (int i = 0; i < files.Count; i++)
            {
                photos.Add(AddCourtPhoto(courtId, files[i]));
                _context.SaveChanges();
            }
            await _commonService.AddCourtPhotosActivity(UserActivityTypeEnum.AddCourtPhotos, courtId, photos.ToArray());
            return photos;
        }

        public async Task DeletePhotoAsync(long courtId, long photoId)
        {
            CourtPhotoModel courtPhoto = await _context.CourtPhotos.Include(p2 => p2.Photo).Include(p => p.Court)
                .SingleOrDefaultAsync(p => p.CourtId == courtId && p.PhotoId == photoId);
            if (courtPhoto == null || courtPhoto.Photo == null)
            {
                throw new DribblyInvalidOperationException("Tried to delete non-existing photo. Photo ID: " + photoId,
                    friendlyMessageKey: "app.Error_DeletePhotoNotFound");
            }
            else if (courtPhoto.Court == null)
            {
                throw new DribblyInvalidOperationException
                    ("Tried to delete a photo associated to nonexistent court. Photo ID: " + photoId);
            }
            else
            {
                if (courtPhoto.Court.OwnerId == _securityUtility.GetAccountId().Value || AuthenticationService.HasPermission(CourtPermission.DeletePhotoNotOwned))
                {
                    courtPhoto.Photo.DateDeleted = DateTime.UtcNow;
                    _context.CourtPhotos.AddOrUpdate(courtPhoto);
                    _context.SaveChanges();
                    await _commonService.AddCourtPhotosActivity(UserActivityTypeEnum.DeleteCourtPhotos, courtId, courtPhoto.Photo);
                }
                else
                {
                    throw new DribblyForbiddenException
                        ("Authorization failed when trying to delete court photo. Photo ID: " + photoId,
                        friendlyMessageKey: "app.Error_NotAllowedToDeletePhoto");
                }
            }
        }

        public IEnumerable<PhotoModel> GetCourtPhotos(long courtId)
        {
            return _context.CourtPhotos.Include(p1 => p1.Photo)
                .Where(p => p.CourtId == courtId && p.Photo.DateDeleted == null)
                .Select(p => p.Photo).OrderByDescending(x => x.DateAdded);
        }

        private PhotoModel AddCourtPhoto(long courtId, HttpPostedFile file)
        {
            string uploadPath = _fileService.Upload(file, "court/");
            PhotoModel photo = new PhotoModel
            {
                Url = uploadPath,
                UploadedById = _securityUtility.GetAccountId().Value,
                DateAdded = DateTime.Now
            };
            _context.Photos.Add(photo);
            _context.CourtPhotos.Add(new CourtPhotoModel
            {
                CourtId = courtId,
                PhotoId = photo.Id
            });

            return photo;
        }

        #endregion

        #region Court Videos

        public async Task<IEnumerable<VideoModel>> GetCourtVideosAsync(long courtId)
        {
            CourtModel court = await _dbSet.FirstOrDefaultAsync(c => c.Id == courtId);

            if (court == null)
            {
                throw new DribblyInvalidOperationException
                    ("Tried to retrieve video for nonexistent court " + courtId.ToString(),
                    friendlyMessageKey: "app.Error_CouldNotRetrieveVideosCourtNotFound");
            }

            return _context.CourtVideos.Include(v => v.Court).Where(v => v.CourtId == courtId && v.Video.DateDeleted == null)
                .Select(v => v.Video).OrderByDescending(v => v.DateAdded);
        }

        public async Task<VideoModel> AddVideoAsync(long courtId, VideoModel video, HttpPostedFile file)
        {
            CourtModel court = await GetCourtAsync(courtId);

            if (court == null)
            {
                throw new DribblyInvalidOperationException
                    ("Tried to upload a video for nonexistent court " + courtId.ToString(),
                    friendlyMessageKey: "app.Error_CouldNotUploadVideoCourtNotFound");
            }

            if (court.OwnerId == _securityUtility.GetAccountId().Value || AuthenticationService.HasPermission(CourtPermission.AddVideoNotOwned))
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        AddCourtVideo(courtId, video, file);
                        _context.SaveChanges();
                        transaction.Commit();
                        await _commonService.AddCourtVideosActivity(UserActivityTypeEnum.AddCourtVideos, courtId, video);
                        return video;
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw e;
                    }
                }
            }
            else
            {
                throw new DribblyForbiddenException
                    (string.Format("Authorization failed when trying to upload a video to court with ID {0}", court.Id),
                   friendlyMessageKey: "app.Error_UploadCourtVideoNotAuthorized");
            }
        }

        public async Task DeleteCourtVideoAsync(long courtId, long videoId)
        {
            CourtVideoModel courtVideo = await _context.CourtVideos.Include(p2 => p2.Video).Include(p => p.Court)
                .SingleOrDefaultAsync(p => p.CourtId == courtId && p.VideoId == videoId);
            if (courtVideo == null || courtVideo.Video == null)
            {
                throw new DribblyInvalidOperationException
                    ("Tried to delete nonexistent court video. Video ID: " + videoId.ToString(),
                    friendlyMessageKey: "app.Error_DeleteCourtVideoVideoNotFound");
            }
            else if (courtVideo.Court == null)
            {
                throw new DribblyInvalidOperationException
                    ("Tried to delete video associated to nonexistent court " + courtId.ToString(),
                    friendlyMessageKey: "app.Error_DeleteCourtVideoCourtNotFound");
            }
            else
            {
                var currentAccountId = _securityUtility.GetAccountId().Value;
                if (courtVideo.Court.OwnerId == currentAccountId || courtVideo.Video.AddedBy == currentAccountId ||
                    AuthenticationService.HasPermission(CourtPermission.DeleteVideoNotOwned))
                {
                    courtVideo.Video.DateDeleted = DateTime.UtcNow;
                    _context.CourtVideos.AddOrUpdate(courtVideo);
                    _context.SaveChanges();
                    await _commonService.AddCourtVideosActivity(UserActivityTypeEnum.DeleteCourtVideos, courtId, courtVideo.Video);
                }
                else
                {
                    throw new DribblyForbiddenException("Authorization failed when trying to delete court video. Video ID: " + videoId.ToString(),
                        friendlyMessageKey: "app.Error_DeleteCourtVideoUnauthorized");
                }
            }
        }

        private VideoModel AddCourtVideo(long courtId, VideoModel video, HttpPostedFile file)
        {
            string uploadPath = _fileService.Upload(file, "video/");
            video.Src = uploadPath;
            video.AddedBy = _securityUtility.GetAccountId().Value;
            video.DateAdded = DateTime.UtcNow;
            video.Size = file.ContentLength;
            video.Type = file.ContentType;

            _context.Videos.Add(video);
            _context.CourtVideos.Add(new CourtVideoModel
            {
                CourtId = courtId,
                VideoId = video.Id
            });

            return video;
        }

        #endregion
    }

    public interface ICourtsService
    {
        Task<IEnumerable<CourtDetailsViewModel>> GetAllActiveAsync();

        Task<CourtDetailsViewModel> GetCourtAsync(long id);

        IEnumerable<BookingModel> GetCourtBookings(long courtId);

        Task<IEnumerable<GameModel>> GetCourtGamesAsync(long courtId);

        IEnumerable<PhotoModel> GetCourtPhotos(long courtId);

        Task<IEnumerable<VideoModel>> GetCourtVideosAsync(long courtId);

        Task<CourtReviewModalModel> GetCodeReviewModalAsync(long courtId);

        Task DeleteCourtAsync(long courtId);

        Task<long> RegisterAsync(CourtModel court);

        Task<IEnumerable<PhotoModel>> AddPhotosAsync(long courtId);

        Task UpdateCourtAsync(CourtModel court);

        Task UpdateCourtPropertiesAsync(GenericEntityUpdateInputModel input);

        Task UpdateCourtPhoto(long courtId);

        Task DeletePhotoAsync(long courtId, long photoId);

        Task<IEnumerable<CourtModel>> FindActiveCourtsAsync(CourtSearchInputModel input);

        Task<VideoModel> AddVideoAsync(long courtId, VideoModel video, HttpPostedFile file);

        Task DeleteCourtVideoAsync(long courtId, long videoId);

        Task<FollowResultModel> FollowCourtAsync(long courtId, bool isFollowing);

        #region Reviews

        Task<IEnumerable<CourtReviewModel>> GetReviewsAsync(long courtId);

        Task SubmitReviewAsync(CourtReviewModel review);

        #endregion

    }
}