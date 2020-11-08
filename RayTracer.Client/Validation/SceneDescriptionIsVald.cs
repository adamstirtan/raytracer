using System.ComponentModel.DataAnnotations;

namespace RayTracer.Client.Validation
{
    public class SceneDescriptionIsValid : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return new ValidationResult("Validation message to user.",
                new[] { validationContext.MemberName });
        }
    }
}