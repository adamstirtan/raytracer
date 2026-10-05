using System.Numerics;

namespace RayTracer.Core.Materials;

public class Material
{
    public Material(Vector3 color)
        : this(color, 0f, 0f, 0f)
    { }

    public Material(Vector3 color, float diffuse, float reflection, float specular)
    {
        Color = color;
        Diffuse = diffuse;
        Reflection = reflection;
        Specular = specular;
    }

    public Vector3 Color { get; set; }
    public float Diffuse { get; set; }
    public float Specular { get; set; }
    // Optional highlight exponent; unspecified preserves existing scene appearance.
    public float? Shininess { get; set; }
    public float Reflection { get; set; }
    public float Emission { get; set; }
}
