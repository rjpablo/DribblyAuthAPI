using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Dribbly.Core.Utilities;
using DribblyAuthAPI.Models;
using DribblyAuthAPI.Models.Courts;
using DribblyAuthAPI.Models.Games;

namespace DribblyAuthAPI.Services
{
    public class CourtsService : BaseService<CourtModel>, ICourtsService
    {
        IAuthContext _context;
        HttpContextBase _httpContext;
        ISecurityUtility _securityUtility;
        IFileService _fileService;

        public CourtsService(IAuthContext context,
            HttpContextBase httpContext,
            ISecurityUtility securityUtility,
            IFileService fileService) : base(context.Courts)
        {
            _context = context;
            _httpContext = httpContext;
            _securityUtility = securityUtility;
            _fileService = fileService;
        }

        public IEnumerable<CourtModel> GetAll()
        {
            return All();
        }

        public CourtModel GetCourt(long id)
        {
            return GetById(id);
        }

        public long Register(CourtModel court)
        {
            court.OwnerId = _securityUtility.GetUserId();
            Add(court);
            _context.SaveChanges();
            return court.Id;
        }

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

        public void UpdateCourtPhoto(long courtId)
        {
            HttpFileCollection files = HttpContext.Current.Request.Files;
            string uploadPath = _fileService.Upload(files[0], "court/");
            CourtModel court = GetById(courtId);
            court.PrimaryPhotoUrl = uploadPath;
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
            UpdateCourt(court);
        }

        public void UpdateCourt(CourtModel court)
        {
            Update(court);
            _context.SaveChanges();
        }

        public IEnumerable<PhotoModel> GetCourtPhotos(long courtId)
        {
            CourtModel court = _context.Courts.Include(c => c.Photos.Select(p => p.Photo)).FirstOrDefault(_court => _court.Id == courtId);
            return court.Photos.Select(p => p.Photo).OrderByDescending(x => x.DateAdded);
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

        public IEnumerable<GameModel> GetCourtGames(long courtId)
        {
            var games = _context.Games.Where(g => g.CourtId == courtId).ToList();
            return games;
        }
    }
}