using Bloggi.Models;

namespace Bloggi.ViewModels;

public class EditUserViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public IFormFile ProfileImage { get; set; }
    public string Brief { get; set; }
    public string? ProfileImageUrl { get; set; }=string.Empty;
    public List<SocialLinksViewModel> SocialLinks { get; set; }=new List<SocialLinksViewModel>();
}
