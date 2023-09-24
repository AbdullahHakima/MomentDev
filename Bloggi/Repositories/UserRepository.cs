using Bloggi.Data;
using Bloggi.Models;

namespace Bloggi.Repositories
{
    public class UserRepository :BaseRepository<User>
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public string GetImageUrl(byte[] image)
        {
            return $"data:image/jpeg;base64,{Convert.ToBase64String(image)}";
        }
    }
}
