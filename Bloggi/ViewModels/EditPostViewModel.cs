using Bloggi.Validations;

namespace Bloggi.ViewModels
{
    public class EditPostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Brief { get; set; }
        public string Content { get; set; }
        public DateTime UpdateOn { get; set; }= DateTime.Now;
        [ImageExtenstion()]
        [ImageSize()]
        public IFormFile Image { get; set; }
        public int ReadingTime { get; set; }
        //public virtual List<Tag> Tags { get; set; }
        public List<int> SelectedTagIds { get; set; }
        
    }
}
