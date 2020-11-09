using System.Drawing;

namespace RayTracer.Core.Materials
{
    public class Material
    {
        public Color Color { get; set; }

        public float Diffuse { get; set; }

        public float Specular { get; set; }

        public float Reflection { get; set; }
    }
}