using Dribbly.Model.Comments;
using Dribbly.Model.Entities;
using Dribbly.Service.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Comments")]
    [Authorize]
    public class CommentsController : BaseController
    {
        private ICommentsService _service = null;

        public CommentsController(ICommentsService service) : base()
        {
            _service = service;
        }

        [HttpPost, AllowAnonymous]
        [Route("GetComments")]
        public async Task<IEnumerable<CommentModel>> GetComments([FromBody]GetCommentsInputModel input)
        {
            return await _service.GetComments(input);
        }

        //POSTs
        [HttpPost]
        [Route("AddComment")]
        public async Task<CommentModel> AddComment([FromBody]AddCommentInputModel input)
        {
            return await _service.AddComment(input);
        }
    }
}
