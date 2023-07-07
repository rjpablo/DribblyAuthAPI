using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Dribbly.Service.Services
{
    public class FileService: IFileService
    {
        public string Upload(HttpPostedFile file, string directoryPath)
        {            

            string uploadBasePath = "Files/Upload/";
            string hostName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/";
            HttpFileCollection files = HttpContext.Current.Request.Files;
            string physicalFolderPath = HttpContext.Current.Server.MapPath("~/" + uploadBasePath + directoryPath);

            string uploadedFilePath = "";

            try
            {
                if (!Directory.Exists(physicalFolderPath))
                {
                    Directory.CreateDirectory(physicalFolderPath);
                }

                string ext = Path.GetExtension(file.FileName);
                string uploadedFileName;

                do
                {
                    uploadedFileName = DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + ext;
                    uploadedFilePath = physicalFolderPath + uploadedFileName;
                } while (File.Exists(uploadedFilePath));

                file.SaveAs(uploadedFilePath);

                return hostName + uploadBasePath + directoryPath + uploadedFileName;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public interface IFileService
    {
        string Upload(HttpPostedFile file, string directoryPath);
    }
}