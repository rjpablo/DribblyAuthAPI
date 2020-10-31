using Dribbly.Authentication.Services;
using Dribbly.Core.Enums.Permissions;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Bookings;
using Dribbly.Model.Courts;
using Dribbly.Model.Games;
using Dribbly.Model.Shared;
using Dribbly.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Migrations;
using System.Linq;
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

        public CourtsService(IAuthContext context,
            ISecurityUtility securityUtility,
            IFileService fileService,
            ICourtsRepository courtsRepo,
            IAccountRepository accountRepo) : base(context.Courts)
        {
            _context = context;
            _securityUtility = securityUtility;
            _fileService = fileService;
            _courtsRepo = courtsRepo;
            _accountRepo = accountRepo;
        }

        #region Generic Court methods

        public async Task<IEnumerable<CourtDetailsViewModel>> GetAllAsync()
        {
            CourtModel[] courts = await _context.Courts.Include(p => p.PrimaryPhoto).ToArrayAsync();
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
            string currentUserId = _securityUtility.GetUserId();
            result.IsFollowed = _context.CourtFollowings.Any(f => f.CourtId == id && f.FollowedById == currentUserId);

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
                    string userId = _securityUtility.GetUserId();
                    Model.Bookings.BookingModel mostRecentBooking = await context.Bookings.Where(e => e.CourtId == courtId && e.BookedById == userId &&
                    e.Start < DateTime.UtcNow && e.End < DateTime.UtcNow && e.Status == Enums.BookingStatusEnum.Approved && !e.HasReviewed)
                        .OrderByDescending(e => e.End).FirstOrDefaultAsync();
                    return mostRecentBooking;
                }
                return null;
            }
        }

        public async Task<CourtReviewModalModel> GetCodeReviewModalAsync(long courtId)
        {
            Model.Bookings.BookingModel mostRecentBooking = await GetCurrentUserMostRecentBooking(courtId);
            if(mostRecentBooking == null)
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

        public async Task<IEnumerable<CourtModel>> FindCourtsAsync(CourtSearchInputModel input)
        {
            return await _courtsRepo.FindCourtsAsync(input);
        }

        public async Task<FollowResultModel> FollowCourtAsync(long courtId, bool isFollowing)
        {
            string currentUserId = _securityUtility.GetUserId();
            FollowResultModel result = new FollowResultModel();
            CourtFollowingModel courtFollowing = await _context.CourtFollowings.SingleOrDefaultAsync(f => f.CourtId == courtId && f.FollowedById == currentUserId);
            result.isAlreadyFollowing = courtFollowing != null;
            result.IsNotCurrentlyFollowing = !result.isAlreadyFollowing;

            if (isFollowing)
            {

                if (!result.isAlreadyFollowing)
                {
                    _context.CourtFollowings.Add(new CourtFollowingModel
                    {
                        CourtId = courtId,
                        FollowedById = currentUserId
                    });
                    await _context.SaveChangesAsync();
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

        public long Register(CourtModel court)
        {
            court.OwnerId = _securityUtility.GetUserId();
            Add(court);
            _context.SaveChanges();
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
                        UploadedById = _securityUtility.GetUserId(),
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
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public void UpdateCourt(CourtModel court)
        {
            if (court.OwnerId == _securityUtility.GetUserId() || AuthenticationService.HasPermission(CourtPermission.UpdateNotOwned))
            {
                Update(court);
                _context.SaveChanges();
            }
            else
            {
                throw new UnauthorizedAccessException("Authorization failed when attempting to update court details.");
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
            review.ReviewedById = _securityUtility.GetUserId();
            CourtModel court = GetById(review.CourtId);
            if (court.OwnerId == review.ReviewedById)
            {
                throw new InvalidOperationException("app.Error_CantRateOwnCourt");
            }
            Model.Bookings.BookingModel reviewBooking = _context.Bookings.SingleOrDefault(e => e.Id == review.BookingId);
            if(reviewBooking == null)
            {
                throw new InvalidOperationException("app.Error_BookingNotFound");
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

        public IEnumerable<PhotoModel> AddPhotos(long courtId)
        {
            HttpFileCollection files = HttpContext.Current.Request.Files;
            List<PhotoModel> photos = new List<PhotoModel>();
            for (int i = 0; i < files.Count; i++)
            {
                photos.Add(AddCourtPhoto(courtId, files[i]));
                _context.SaveChanges();
            }

            return photos;
        }

        public async Task DeletePhotoAsync(long courtId, long photoId)
        {
            CourtPhotoModel courtPhoto = await _context.CourtPhotos.Include(p2 => p2.Photo).Include(p => p.Court)
                .SingleOrDefaultAsync(p => p.CourtId == courtId && p.PhotoId == photoId);
            if (courtPhoto == null || courtPhoto.Photo == null)
            {
                throw new InvalidOperationException("Photo not found.");
            }
            else if (courtPhoto.Court == null)
            {
                throw new ObjectNotFoundException
                    ("The court associated with the photo being deleted was not found.");
            }
            else
            {
                if (courtPhoto.Court.OwnerId == _securityUtility.GetUserId() || AuthenticationService.HasPermission(CourtPermission.DeletePhotoNotOwned))
                {
                    courtPhoto.Photo.DateDeleted = DateTime.UtcNow;
                    _context.CourtPhotos.AddOrUpdate(courtPhoto);
                    _context.SaveChanges();
                }
                else
                {
                    throw new UnauthorizedAccessException("Authorization failed when trying to delete court photo.");
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
                UploadedById = _securityUtility.GetUserId(),
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
                throw new ObjectNotFoundException("No court was found with id " + courtId.ToString());
            }

            return _context.CourtVideos.Include(v => v.Court).Where(v => v.CourtId == courtId && v.Video.DateDeleted == null)
                .Select(v => v.Video).OrderByDescending(v => v.DateAdded);
        }

        public async Task<VideoModel> AddVideoAsync(long courtId, VideoModel video, HttpPostedFile file)
        {
            CourtModel court = await GetCourtAsync(courtId);

            if (court == null)
            {
                throw new ObjectNotFoundException("No court was found with id " + courtId.ToString());
            }

            if (_securityUtility.IsCurrentUser(court.OwnerId) || AuthenticationService.HasPermission(CourtPermission.AddVideoNotOwned))
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        AddCourtVideo(courtId, video, file);
                        _context.SaveChanges();
                        transaction.Commit();
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
                throw new UnauthorizedAccessException
                    (string.Format("Authorization failed when trying to upload a video to court with ID {0}", court.Id));
            }
        }

        public async Task DeleteCourtVideoAsync(long courtId, long videoId)
        {
            CourtVideoModel courtVideo = await _context.CourtVideos.Include(p2 => p2.Video).Include(p => p.Court)
                .SingleOrDefaultAsync(p => p.CourtId == courtId && p.VideoId == videoId);
            if (courtVideo == null || courtVideo.Video == null)
            {
                throw new InvalidOperationException("Video not found.");
            }
            else if (courtVideo.Court == null)
            {
                throw new ObjectNotFoundException
                    ("The court associated with the video being deleted was not found.");
            }
            else
            {
                if (_securityUtility.IsCurrentUser(courtVideo.Court.OwnerId) || _securityUtility.IsCurrentUser(courtVideo.Video.AddedBy) ||
                    AuthenticationService.HasPermission(CourtPermission.DeleteVideoNotOwned))
                {
                    courtVideo.Video.DateDeleted = DateTime.UtcNow;
                    _context.CourtVideos.AddOrUpdate(courtVideo);
                    _context.SaveChanges();
                }
                else
                {
                    throw new UnauthorizedAccessException("Authorization failed when trying to delete court video.");
                }
            }
        }

        private VideoModel AddCourtVideo(long courtId, VideoModel video, HttpPostedFile file)
        {
            string uploadPath = _fileService.Upload(file, "video/");
            video.Src = uploadPath;
            video.AddedBy = _securityUtility.GetUserId();
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
}