using Bloggi.Consts;
using System.ComponentModel.DataAnnotations;

namespace Bloggi.Validations
{
    public class ImageExtenstionAttribute:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var Image = (IFormFile?)value;
            if (!ImageConst.Extenstions.Contains(Path.GetExtension(Image.FileName)))
            {
                return new ValidationResult(ErrorMessage= "\"Only .png , .jpg are allowed for post's image.\"");
            }
            return ValidationResult.Success;
        }
    }
}
