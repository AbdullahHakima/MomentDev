using Bloggi.Models;
using System.Security.Policy;

namespace Bloggi.ViewModels
{
    public class SearchResultViewModel
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }=string.Empty;
        public DateTime CreatedOn { get; set; }
        public int ReadingTime { get; set; } 
        public string Brief { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
       // public List<Tag> tags { get; set; }
    }
}
