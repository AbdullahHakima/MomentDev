using Bloggi.Models;

namespace Bloggi.ViewModels
{
    public class HomePostViewModel
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Brief { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ImageUrl { get; set; }
        public int ReadingTime { get; set; }
        public virtual List<Tag> Tags { get; set; }
    }
}
