using System.Web;

namespace DribblyAuthAPI.Services
{
    public interface IFileService
    {
        string Upload(HttpPostedFile file, string directoryPath);
    }
}