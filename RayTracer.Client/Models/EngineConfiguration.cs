using System.ComponentModel.DataAnnotations;

namespace RayTracer.Client.Models
{
    public class EngineConfiguration
    {
        [Required]
        public string Json { get; set; }
    }
}