using Bloggi.Validations;

namespace Bloggi.ViewModels;

public class CreateUserViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [ImageExtenstion,ImageSize]
    public IFormFile ProfileImage { get; set; }
    public string Brief { get; set; }

    public List<string> SocialLinks { get; set; } = new List<string>();
}
