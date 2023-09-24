using Bloggi.Models;

namespace Bloggi.ViewModels;

public class RelatedPostViewModel
{
	public int Id { get; set; }
	public required string Title { get; set; }
	public required string Brief { get; set; }
	public DateTime CreatedOn { get; set; }
	public required string ImageUrl { get; set; }
	
    public int ReadingTime { get; set; }

}
