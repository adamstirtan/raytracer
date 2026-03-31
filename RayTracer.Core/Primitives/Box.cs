using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Math;

namespace RayTracer.Core.Primitives;

public class Box : Primitive
{
    public Vector3 Min { get; set; }
    public Vector3 Max { get; set; }

    public Box(Vector3 min, Vector3 max, Material material, Texture? texture)
        : base(material, texture)
    {
        Min = min;
        Max = max;
    }

    public override PrimitiveType GetPrimitiveType() => PrimitiveType.Box;

    public override RayIntersection Intersects(Ray ray, ref float distance)
    {
        // Slab method
        float tmin = (Min.X - ray.Origin.X) / ray.Direction.X;
        float tmax = (Max.X - ray.Origin.X) / ray.Direction.X;
        if (tmin > tmax) (tmin, tmax) = (tmax, tmin);

        float tymin = (Min.Y - ray.Origin.Y) / ray.Direction.Y;
        float tymax = (Max.Y - ray.Origin.Y) / ray.Direction.Y;
        if (tymin > tymax) (tymin, tymax) = (tymax, tymin);

        if ((tmin > tymax) || (tymin > tmax)) return RayIntersection.Miss;

        if (tymin > tmin) tmin = tymin;
        if (tymax < tmax) tmax = tymax;

        float tzmin = (Min.Z - ray.Origin.Z) / ray.Direction.Z;
        float tzmax = (Max.Z - ray.Origin.Z) / ray.Direction.Z;
        if (tzmin > tzmax) (tzmin, tzmax) = (tzmax, tzmin);

        if ((tmin > tzmax) || (tzmin > tmax)) return RayIntersection.Miss;

        if (tzmin > tmin) tmin = tzmin;
        if (tzmax < tmax) tmax = tzmax;

        float t = tmin > 1e-6f ? tmin : tmax;
        if (t > 1e-6f && t < distance)
        {
            distance = t;
            return RayIntersection.Hit;
        }

        return RayIntersection.Miss;
    }

    public override Vector3 GetNormal(Vector3 position)
    {
        // Determine closest face
        float dx = System.MathF.Min(System.MathF.Abs(position.X - Min.X), System.MathF.Abs(position.X - Max.X));
        float dy = System.MathF.Min(System.MathF.Abs(position.Y - Min.Y), System.MathF.Abs(position.Y - Max.Y));
        float dz = System.MathF.Min(System.MathF.Abs(position.Z - Min.Z), System.MathF.Abs(position.Z - Max.Z));

        if (dx < dy && dx < dz)
            return position.X - Min.X < position.X - Max.X ? Vector3.UnitX * -1 : Vector3.UnitX;
        else if (dy < dx && dy < dz)
            return position.Y - Min.Y < position.Y - Max.Y ? Vector3.UnitY * -1 : Vector3.UnitY;
        else
            return position.Z - Min.Z < position.Z - Max.Z ? Vector3.UnitZ * -1 : Vector3.UnitZ;
    }

    public override Vector2 GetUV(Vector3 position)
    {
        // Simple mapping based on dominant face
        var normal = GetNormal(position);
        if (normal == Vector3.UnitX || normal == -Vector3.UnitX)
            return new Vector2((position.Z - Min.Z) / (Max.Z - Min.Z), (position.Y - Min.Y) / (Max.Y - Min.Y));
        if (normal == Vector3.UnitY || normal == -Vector3.UnitY)
            return new Vector2((position.X - Min.X) / (Max.X - Min.X), (position.Z - Min.Z) / (Max.Z - Min.Z));
        return new Vector2((position.X - Min.X) / (Max.X - Min.X), (position.Y - Min.Y) / (Max.Y - Min.Y));
    }
}
