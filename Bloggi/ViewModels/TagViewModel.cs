using Bloggi.Models;

namespace Bloggi.ViewModels;

public class TagViewModel
{
	public required string Name { get; set; }
	public required string Description { get; set; }
	public int TagPostCount { get; set; }
	public virtual List<TagPostViewModel> Posts { get; set; }
	public List<Tag> Tags { get; set; }= new List<Tag>();

}
