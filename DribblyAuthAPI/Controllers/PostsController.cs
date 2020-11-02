using Dribbly.Model.Posts;
using Dribbly.Service.Enums;
using Dribbly.Service.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Posts")]
    public class PostsController : BaseController
    {
        private IPostsService _service = null;

        public PostsController(IPostsService service) : base()
        {
            _service = service;
        }

        //GETs
        [HttpPost]
        [Route("GetPosts")]
        public async Task<IEnumerable<PostModel>> GetPosts([FromBody]GetPostsInputModel input)
        {
            return await _service.GetPosts(input);
        }

        //POSTs
        [HttpPost]
        [Route("AddPost")]
        public async Task<PostModel> AddPost([FromBody]AddEditPostInputModel input)
        {
            return await _service.AddPost(input);
        }

        [HttpPost]
        [Route("UpdatePost")]
        public async Task<PostModel> UpdatePost([FromBody]AddEditPostInputModel input)
        {
            return await _service.UpdatePost(input);
        }

        [HttpPost]
        [Route("DeletePost/{id}")]
        public async Task<bool> DeletePost(long Id)
        {
            return await _service.DeletePost(Id);
        }
    }
}
