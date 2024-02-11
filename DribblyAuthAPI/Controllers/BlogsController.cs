using Dribbly.Core.Models;
using Dribbly.Model.Entities.Blogs;
using Dribbly.Service.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Blogs")]
    public class BlogsController : BaseController
    {
        private IBlogsService _service = null;

        public BlogsController(IBlogsService service) : base()
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<BlogModel>> GetBlogs()
        {
            return await _service.GetBlogs();
        }

        [Route("{slug}")]
        public async Task<BlogModel> GetBlog(string slug)
        {
            return await _service.GetBlog(slug);
        }

        [Route("Entity/{slug}")]
        public async Task<IIndexedEntity> GetBlogEntity(string slug)
        {
            return await _service.GetBlogEntity(slug);
        }
    }
}
