using System.Web;

namespace Dribbly.Service.Services
{
    public interface IFileService
    {
        string Upload(HttpPostedFile file, string directoryPath);
    }
}