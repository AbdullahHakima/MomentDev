using Bloggi.Consts;
using System.ComponentModel.DataAnnotations;

namespace Bloggi.Validations
{
    public class ImageSizeAttribute:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var Image = (IFormFile?)value;
            if (Image.Length>ImageConst.MaxSize)
            {
                return new ValidationResult(ErrorMessage = "The max allowed size for poster is 2Mb.");
            }
            return ValidationResult.Success;
        }

    }
}
