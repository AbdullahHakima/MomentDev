namespace Bloggi.Models;

public class SocialLinks
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Url { get; set; }
	public virtual User User { get; set; }
}
