using System.Numerics;

using RayTracer.Core.Materials;
using RayTracer.Core.Math;

namespace RayTracer.Core.Primitives;

public class Plane : Primitive
{
    public Vector3 Normal { get; set; }
    public float D { get; set; }

    public Plane(Vector3 normal, float d, Material material, Texture? texture)
        : base(material, texture)
    {
        Normal = normal;
        D = d;
    }

    public override PrimitiveType GetPrimitiveType()
    {
        return PrimitiveType.Plane;
    }

    public override RayIntersection Intersects(Ray ray, ref float distance)
    {
        float d = Vector3.Dot(Normal, ray.Direction);
        float dist = -(Vector3.Dot(Normal, ray.Origin) + D) / d;

        if (dist > 0)
        {
            if (dist < distance)
            {
                distance = dist;
                return RayIntersection.Hit;
            }
        }

        return RayIntersection.Miss;
    }

    public override Vector3 GetNormal(Vector3 position)
    {
        return Normal;
    }

    public override Vector2 GetUV(Vector3 position)
    {
        // Simple planar UV mapping using world X and Z coordinates (assumes plane normal is Y up)
        // Adjust scale to control tiling density on the plane
        float scale = 0.5f; // smaller values -> more repeats

        float u = position.X * scale;
        float v = position.Z * scale;

        // Wrap into [0,1]
        u = u - MathF.Floor(u);
        v = v - MathF.Floor(v);

        return new Vector2(u, v);
    }
}