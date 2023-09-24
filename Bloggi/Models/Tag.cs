namespace Bloggi.Models;

public class Tag
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }

    public virtual IEnumerable<Post> Posts { get; set; }
    public virtual List<PostTag> PostTags { get; set; }
}
