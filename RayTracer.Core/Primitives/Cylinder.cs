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

    public override PrimitiveType GetPrimitiveType() => PrimitiveType.Cylinder;

    public override RayIntersection Intersects(Ray ray, ref float distance)
    {
        // Cylinder aligned on Y axis, finite with caps
        Vector3 d = ray.Direction;
        Vector3 o = ray.Origin - Center;

        float a = d.X * d.X + d.Z * d.Z;
        float b = 2 * (o.X * d.X + o.Z * d.Z);
        float c = o.X * o.X + o.Z * o.Z - Radius * Radius;

        float closestDistance = distance;
        float halfHeight = Height / 2f;
        bool hit = false;

        void CheckSide(float t)
        {
            if (t <= 1e-6f || t >= closestDistance) return;
            float y = o.Y + d.Y * t;
            if (y < -halfHeight || y > halfHeight) return;
            closestDistance = t;
            hit = true;
        }

        void CheckCap(float y)
        {
            float t = (y - o.Y) / d.Y;
            if (t <= 1e-6f || t >= closestDistance) return;
            Vector3 p = o + d * t;
            if (p.X * p.X + p.Z * p.Z > Radius * Radius) return;
            closestDistance = t;
            hit = true;
        }

        // Axis-parallel rays have no side roots; still test the caps.
        if (a > 0f)
        {
            float disc = b * b - 4 * a * c;
            if (disc >= 0f)
            {
                float sqrt = System.MathF.Sqrt(disc);
                CheckSide((-b - sqrt) / (2 * a));
                CheckSide((-b + sqrt) / (2 * a));
            }
        }

        if (d.Y != 0f)
        {
            CheckCap(-halfHeight);
            CheckCap(halfHeight);
        }

        if (!hit) return RayIntersection.Miss;
        distance = closestDistance;
        return RayIntersection.Hit;
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
