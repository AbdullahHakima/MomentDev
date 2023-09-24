namespace Bloggi.ViewModels;

public class TagPostViewModel
{
	public int Id { get; set; }
	public required string Title { get; set; }
	public required string Brief { get; set; }
	public DateTime CreatedOn { get; set; }
	public string ImageUrl { get; set; }
	public int ReadingTime { get; set; }
}
