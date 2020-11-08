using System.ComponentModel.DataAnnotations;

namespace RayTracer.Client.Models
{
    public class SceneDescription
    {
        [Required]
        public string Name { get; set; }

        public string Slug { get; set; }

        [Required]
        public string Json { get; set; }
    }
}