using SixLabors.ImageSharp;

namespace RayTracer.Core.Materials
{
    public class Material
    {
        public Material(Color color)
            : this(color, 0f, 0f, 0f)
        { }

        public Material(Color color, float diffuse, float reflection, float specular)
        {
            Color = color;
            Diffuse = diffuse;
            Reflection = reflection;
            Specular = specular;
        }

        public Color Color { get; set; }

        public float Diffuse { get; set; }

        public float Specular { get; set; }

        public float Reflection { get; set; }

        public static Material Black => new Material(Color.Black);
        public static Material White => new Material(Color.White);
        public static Material Gray => new Material(Color.Gray);
    }
}