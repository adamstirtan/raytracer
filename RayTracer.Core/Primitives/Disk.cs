using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Math;

namespace RayTracer.Core.Primitives;

public class Disk : Primitive
{
    public Vector3 Center { get; set; }
    public Vector3 Normal { get; set; }
    public float Radius { get; set; }

    public Disk(Vector3 center, Vector3 normal, float radius, Material material, Texture? texture)
        : base(material, texture)
    {
        Center = center;
        Normal = Vector3.Normalize(normal);
        Radius = radius;
    }

    public override PrimitiveType GetPrimitiveType() => PrimitiveType.Plane;

    public override RayIntersection Intersects(Ray ray, ref float distance)
    {
        float denom = Vector3.Dot(Normal, ray.Direction);
        if (System.MathF.Abs(denom) < 1e-6f) return RayIntersection.Miss;

        float t = Vector3.Dot(Center - ray.Origin, Normal) / denom;
        if (t > 1e-6f && t < distance)
        {
            Vector3 p = ray.Origin + ray.Direction * t;
            if (Vector3.DistanceSquared(p, Center) <= Radius * Radius)
            {
                distance = t;
                return RayIntersection.Hit;
            }
        }

        return RayIntersection.Miss;
    }

    public override Vector3 GetNormal(Vector3 position) => Normal;

    public override Vector2 GetUV(Vector3 position)
    {
        var local = position - Center;
        return new Vector2((local.X / Radius + 1) * 0.5f, (local.Z / Radius + 1) * 0.5f);
    }
}
