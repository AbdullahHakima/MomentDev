using System.ComponentModel.DataAnnotations;

namespace Bloggi.ViewModels
{
	public class SubscriberModelView
	{
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }
	}
}
