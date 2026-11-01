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
    /// <summary>Index of refraction (1.0 = air, ~1.5 = glass). Only used when <see cref="Transmission"/> is greater than 0.</summary>
    public float Ior { get; set; } = 1f;
    /// <summary>Fraction of light that may pass through the material (0 = opaque, 1 = fully transmissive).</summary>
    public float Transmission { get; set; }
}
