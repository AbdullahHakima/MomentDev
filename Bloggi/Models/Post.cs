using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace Bloggi.Models
{
    public class Post
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Brief { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set;}
        public required string Content { get; set; }
        public required byte[] Image { get; set; } 
        public int ReadingTime { get; set; }
        //variable for Image post uploaded outside the server
        [DataType(DataType.ImageUrl)]
        public string ImageUrlOutServer { get; set; }
		public virtual ICollection<Tag> Tags { get; set; }=new List<Tag>();
        public virtual List<PostTag> PostTags { get; set; }

        public virtual User User { get; set; }
        public int UserId { get; set; }
        
    }
}
