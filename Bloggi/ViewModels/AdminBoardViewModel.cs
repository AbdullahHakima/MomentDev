using Bloggi.Models;

namespace Bloggi.ViewModels
{
    public class AdminBoardViewModel
    {
        public List<Post> posts { get; set; } 
        public List<Tag> tags { get; set; }
    }
}
