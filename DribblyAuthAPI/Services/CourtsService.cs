using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dribbly.Core.Utilities;
using DribblyAuthAPI.Models;
using DribblyAuthAPI.Models.Courts;

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
            IFileService fileService) :base(context.Courts)
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

        public void UpdateCourtPhoto(long courtId)
        {
            HttpFileCollection files = HttpContext.Current.Request.Files;
            string uploadPath = _fileService.Upload(files[0], "court/");
            CourtModel court = GetById(courtId);
            court.PrimaryPhotoUrl = uploadPath;
            UpdateCourt(court);
        }

        public void UpdateCourt(CourtModel court)
        {
            Update(court);
            _context.SaveChanges();
        }

    }
}