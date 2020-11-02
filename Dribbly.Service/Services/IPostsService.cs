using Dribbly.Model.Games;
using Dribbly.Model.Posts;
using Dribbly.Service.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public interface IPostsService
    {
        Task<IEnumerable<PostModel>> GetPosts(GetPostsInputModel input);

        Task<PostModel> AddPost(AddEditPostInputModel input);
    }
}