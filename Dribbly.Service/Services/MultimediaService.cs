using Dribbly.Core.Exceptions;
using Dribbly.Core.Models;
using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Courts;
using Dribbly.Model.Entities;
using Dribbly.Model.Tournaments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Dribbly.Service.Services
{
    public class MultimediaService : BaseEntityService<TournamentModel>, IMultimediaService
    {
        IAuthContext _context;
        ISecurityUtility _securityUtility;
        private readonly IFileService _fileService;

        public MultimediaService(IAuthContext context,
            IFileService fileService,
            ISecurityUtility securityUtility
            ) : base(context.Tournaments, context)
        {
            _context = context;
            _securityUtility = securityUtility;
            _fileService = fileService;
        }

        public async Task<IEnumerable<MultimediaModel>> UploadAsync()
        {
            var accountId = _securityUtility.GetAccountId().Value;
            List<MultimediaModel> result = new List<MultimediaModel>();

            HttpFileCollection files = HttpContext.Current.Request.Files;
            if (files.Count == 0)
            {
                throw new DribblyInvalidOperationException("No files to upload",
                    friendlyMessage: "The request did not contain the file to be uploaded");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        string uploadPath = _fileService.Upload(files[i], $"{accountId}/messageFiles/");
                        MultimediaModel photo = new MultimediaModel
                        {
                            Url = uploadPath,
                            Type = Core.Enums.MultimediaTypeEnum.Photo,
                            DateAdded = DateTime.UtcNow
                        };
                        _context.Multimedia.Add(photo);
                        await _context.SaveChangesAsync();
                        //TODO: log user activity
                        result.Add(photo);
                    }

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
            return result;
        }
    }

    public interface IMultimediaService
    {
        Task<IEnumerable<MultimediaModel>> UploadAsync();
    }
}