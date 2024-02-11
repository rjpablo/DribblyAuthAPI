using Dribbly.Core.Models;
using Dribbly.Model;
using Dribbly.Model.Entities.Blogs;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;

namespace Dribbly.Service.Services
{
    public class BlogsService : BaseEntityService<BlogModel>, IBlogsService
    {
        IAuthContext _context;

        public BlogsService(IAuthContext context) : base(context.Blogs, context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BlogModel>> GetBlogs()
        {
            var blogs = await _context.Blogs.OrderByDescending(b=>b.DateAdded).ToListAsync();
            return blogs;
        }

        public async Task<BlogModel> GetBlog(string slug)
        {
            var blog = SingleOrDefault(b => b.Slug == slug);
            if (blog != null)
            {
                blog.VisitCount++;
                await _context.SaveChangesAsync();
            }
            return blog;
        }

        public async Task<IIndexedEntity> GetBlogEntity(string slug)
        {
            return SingleOrDefault(b => b.Slug == slug);
        }
    }

    public interface IBlogsService
    {
        Task<BlogModel> GetBlog(string slug);
        Task<IEnumerable<BlogModel>> GetBlogs();
        Task<IIndexedEntity> GetBlogEntity(string slug);
    }
}