using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace RayTracer.Client.Validation
{
    public class SceneDescriptionIsValid : ValidationAttribute
    {
        private const string schemaJson = @"{
            'description': 'Scene description',
            'type': 'object',
            'properties': {
                'name': {'type': 'string'},
                'slug': {'type': 'string'},
                'json': {'type': 'string'}
            }
        }";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("JSON cannot be null", new[] { validationContext.MemberName });
            }

            JSchema schema = JSchema.Parse(schemaJson);
            JObject obj;

            try
            {
                obj = JObject.Parse(value.ToString());
            }
            catch (JsonReaderException jsonReaderException)
            {
                return new ValidationResult(jsonReaderException.Message, new[] { validationContext.MemberName });
            }

            if (obj.IsValid(schema))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("JSON is invalid", new[] { validationContext.MemberName });
        }
    }
}