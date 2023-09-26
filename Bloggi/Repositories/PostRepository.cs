using Bloggi.Contracts;
using Bloggi.Data;
using Bloggi.Models;
using Bloggi.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Bloggi.Repositories;

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
    public async Task<List<SearchResultViewModel>> search(SearchViewModel model)
    {
        IQueryable<Post> query = _context.Posts.Include(p=>p.Tags);
        if (!string.IsNullOrEmpty(model.Content))
        {
            query = query.Where(m => m.Tags.Any(tag => tag.Name.Contains(model.Content)));
        }
        if (!string.IsNullOrEmpty(model.Content))
        {
            var posts=query.Where(m => m.Title.Contains(model.Content));
            if (posts != null)
                foreach (var post in posts)
                    query.Append(post);
        }
        
        var result = query.Select(entity => new SearchResultViewModel
        {
            PostTitle = entity.Title,
            Brief = entity.Brief,
            CreatedOn = entity.CreatedOn,
            ImageUrl = entity.ImageUrl,
            PostId = entity.Id,
            ReadingTime=entity.ReadingTime,
            //tags=entity.Tags.ToList(),
        }).ToList();
        return result;

    }
}
