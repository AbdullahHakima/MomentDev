using System.ComponentModel.DataAnnotations;

namespace Bloggi.ViewModels
{
    public class UserSignUpViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public IFormFile Image { get; set; }
        [DataType(DataType.ImageUrl)]
        public string ImageUrlOutServer { get; set; }
        public string Brief { get; set; }
    }
}
