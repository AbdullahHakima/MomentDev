using System.ComponentModel.DataAnnotations;

namespace Bloggi.Models;

public class Subscriber
{
	public int Id { get; set; }
	[DataType(DataType.EmailAddress)]
	public string Email { get; set; }
}
