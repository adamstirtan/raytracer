using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Math;

namespace RayTracer.Core.Primitives;

public class Cylinder : Primitive
{
    public Vector3 Center { get; set; }
    public float Radius { get; set; }
    public float Height { get; set; }

    public Cylinder(Vector3 center, float radius, float height, Material material, Texture? texture)
        : base(material, texture)
    {
        Center = center;
        Radius = radius;
        Height = height;
    }

    public override PrimitiveType GetPrimitiveType() => PrimitiveType.Box; // reuse enum for now

    public override RayIntersection Intersects(Ray ray, ref float distance)
    {
        // Cylinder aligned on Y axis, finite with caps
        Vector3 d = ray.Direction;
        Vector3 o = ray.Origin - Center;

        float a = d.X * d.X + d.Z * d.Z;
        float b = 2 * (o.X * d.X + o.Z * d.Z);
        float c = o.X * o.X + o.Z * o.Z - Radius * Radius;

        float disc = b * b - 4 * a * c;
        if (disc < 0) return RayIntersection.Miss;

        float sqrt = System.MathF.Sqrt(disc);
        float t0 = (-b - sqrt) / (2 * a);
        float t1 = (-b + sqrt) / (2 * a);

        float t = float.MaxValue;
        if (t0 > 1e-6f) t = t0;
        else if (t1 > 1e-6f) t = t1;

        if (t < float.MaxValue)
        {
            float y = o.Y + d.Y * t;
            if (y >= -Height/2 && y <= Height/2 && t < distance)
            {
                distance = t;
                return RayIntersection.Hit;
            }
        }

        // caps
        if (System.MathF.Abs(d.Y) > 1e-6f)
        {
            float tcap1 = (-Height/2 - o.Y) / d.Y;
            float tcap2 = (Height/2 - o.Y) / d.Y;
            float tcap = tcap1 > 1e-6f ? tcap1 : (tcap2 > 1e-6f ? tcap2 : float.MaxValue);
            if (tcap < distance)
            {
                Vector3 p = o + d * tcap;
                if (p.X*p.X + p.Z*p.Z <= Radius*Radius)
                {
                    distance = tcap;
                    return RayIntersection.Hit;
                }
            }
        }

        return RayIntersection.Miss;
    }

    public override Vector3 GetNormal(Vector3 position)
    {
        var local = position - Center;
        if (System.MathF.Abs(local.Y - Height/2) < 1e-4f) return Vector3.UnitY;
        if (System.MathF.Abs(local.Y + Height/2) < 1e-4f) return -Vector3.UnitY;
        return Vector3.Normalize(new Vector3(local.X, 0, local.Z));
    }

    public override Vector2 GetUV(Vector3 position)
    {
        var local = position - Center;
        float u = 0.5f + (float)(System.Math.Atan2(local.Z, local.X) / (2 * System.Math.PI));
        float v = (local.Y + Height/2) / Height;
        return new Vector2(u, v);
    }
}
