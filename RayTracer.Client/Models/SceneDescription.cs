using System.ComponentModel.DataAnnotations;

using RayTracer.Client.Validation;

namespace RayTracer.Client.Models
{
    public class SceneDescription
    {
        [Required]
        public string Name { get; set; }

        public string Slug { get; set; }

        [Required]
        [SceneDescriptionIsValid]
        public string Json { get; set; }
    }
}