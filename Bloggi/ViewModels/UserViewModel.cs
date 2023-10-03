using Bloggi.Models;
using System.ComponentModel.DataAnnotations;

namespace Bloggi.ViewModels;

public class UserViewModel
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
	[DataType(DataType.ImageUrl)]
	public string ProfileImageUrl { get; set; }
	public string Brief { get; set; }

	public List<string> SocialLinks { get; set; } = new List<string>();
	public List<Post> Posts { get; set; }
	public List<HomePostViewModel> UserPostsVM { get; set; }= new List<HomePostViewModel>();
}
