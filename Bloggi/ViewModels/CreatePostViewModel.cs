using Bloggi.Models;
using Bloggi.Validations;
using System.ComponentModel.DataAnnotations;

namespace Bloggi.ViewModels;
public class CreatePostViewModel
{
    public string Title { get; set; }
    public string Brief { get; set; }
    public string Content { get; set; }
    [ImageExtenstion()]
    [ImageSize()]
    public IFormFile Image { get; set; }
    [DataType(DataType.ImageUrl)]
    public string UploadedImageUrl { get; set; }
    public int ReadingTime { get; set; }
    //public virtual List<Tag> Tags { get; set; }
    public List<int> SelectedTagIds { get; set; }

}

