using Bloggi.Contracts;
using Bloggi.Data;
using Bloggi.Models;
using Microsoft.EntityFrameworkCore;

namespace Bloggi.Repositories
{
    public class PostRepository : BaseRepository<Post>
    {
        private readonly ApplicationDbContext _context;
        public PostRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GetAllRelatedToTag(Post viewedPost)
        {
            var tagIds=viewedPost.Tags.Select(t=>t.Id).ToList();
            return await _context.Posts.Include(p=>p.Tags).Where(p=>p.Tags.Any(t=>tagIds.Contains(t.Id))).OrderByDescending(p=>p.CreatedOn).ToListAsync();
        }
        public string GetImageUrl(byte[] image)
        { 
            //remember to refactore this to take IFormFile not array of byte
            return $"data:image/jpeg;base64,{Convert.ToBase64String(image)}";
        }
    }
}
