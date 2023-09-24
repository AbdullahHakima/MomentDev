using Bloggi.Models;

namespace Bloggi.ViewModels;
    public class PostViewModel
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Brief { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public required string Content { get; set; }
        public required string ImageUrl { get; set; }
        public  required string UserImageUrl { get; set; }
        public List<Tag> Tags { get; set; }= new List<Tag>();
        public int ReadingTime { get; set; }
        public virtual User User { get; set; }
        public List<Post> Posts { get; set; }
        public int RelatedPostCount { get; set; }
       
}

