using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace Bloggi.Models;

public class User:IdentityUser<int>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public byte[] ProfileImage { get; set; }
    public string Brief { get; set; }
    [DataType(DataType.ImageUrl)]
    public string ProfileImageUrl { get; set; }
    
    public virtual List<SocialLinks> SocialLinks { get; set; } = new List<SocialLinks>();
    public virtual List<Post> Posts { get; set; }

    public static implicit operator IdentityUser?(User? v)
    {
        throw new NotImplementedException();
    }
}
