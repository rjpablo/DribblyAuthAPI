﻿using Dribbly.Model.Enums;
using Dribbly.Model.Posts;
using Dribbly.Service.Enums;
using Dribbly.Service.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{    
    [RoutePrefix("api/Posts")]
    [Authorize]
    public class PostsController : BaseController
    {
        private IPostsService _service = null;

        public PostsController(IPostsService service) : base()
        {
            _service = service;
        }

        //GETs
        [HttpGet, AllowAnonymous]
        [Route("GetPost/{postId}")]
        public async Task<PostModel> GetPost(long postId)
        {
            return await _service.GetPostAsync(postId);
        }

        [HttpPost, AllowAnonymous]
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
        [Route("AddReaction")]
        public async Task AddReaction(PostReactionInput input)
        {
            await _service.AddReaction(input);
        }

        [HttpPost]
        [Route("RemoveReaction")]
        public async Task RemoveReaction(PostReactionInput input)
        {
            await _service.RemoveReaction(input);
        }

        [HttpPost]
        [Route("DeletePost/{id}")]
        public async Task<bool> DeletePost(long Id)
        {
            return await _service.DeletePost(Id);
        }
    }
}
