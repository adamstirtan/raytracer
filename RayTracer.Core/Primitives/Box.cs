using System.Numerics;

using RayTracer.Core.Materials;
using RayTracer.Core.Math;

namespace RayTracer.Core.Primitives;

public class Box : Primitive
{
    public Vector3 Min { get; set; }
    public Vector3 Max { get; set; }

    public Box(Vector3 min, Vector3 max, Material material, Texture texture)
        : base(material, texture)
    {
        Min = min;
        Max = max;
    }

    public override PrimitiveType GetPrimitiveType()
    {
        return PrimitiveType.Box;
    }

    public override RayIntersection Intersects(Ray ray, ref float distance)
    {
        float tMin = (Min.X - ray.Origin.X) / ray.Direction.X;
        float tMax = (Max.X - ray.Origin.X) / ray.Direction.X;

        if (tMin > tMax)
        {
            (tMin, tMax) = (tMax, tMin);
        }

        float tyMin = (Min.Y - ray.Origin.Y) / ray.Direction.Y;
        float tyMax = (Max.Y - ray.Origin.Y) / ray.Direction.Y;

        if (tyMin > tyMax)
        {
            (tyMin, tyMax) = (tyMax, tyMin);
        }

        if ((tMin > tyMax) || (tyMin > tMax))
        {
            return RayIntersection.Miss;
        }

        if (tyMin > tMin)
        {
            tMin = tyMin;
        }

        if (tyMax < tMax)
        {
            tMax = tyMax;
        }

        float tzMin = (Min.Z - ray.Origin.Z) / ray.Direction.Z;
        float tzMax = (Max.Z - ray.Origin.Z) / ray.Direction.Z;

        if (tzMin > tzMax)
        {
            (tzMin, tzMax) = (tzMax, tzMin);
        }

        if ((tMin > tzMax) || (tzMin > tMax))
        {
            return RayIntersection.Miss;
        }

        if (tzMin > tMin)
        {
            tMin = tzMin;
        }

        if (tzMax < tMax)
        {
            tMax = tzMax;
        }

        if (tMin < 0)
        {
            tMin = tMax;
            if (tMin < 0)
            {
                return RayIntersection.Miss;
            }
        }

        distance = tMin;
        return RayIntersection.Hit;
    }

    public override Vector3 GetNormal(Vector3 position)
    {
        Vector3 normal = Vector3.Zero;
        if (position.X == Min.X) normal = new Vector3(-1, 0, 0);
        if (position.X == Max.X) normal = new Vector3(1, 0, 0);
        if (position.Y == Min.Y) normal = new Vector3(0, -1, 0);
        if (position.Y == Max.Y) normal = new Vector3(0, 1, 0);
        if (position.Z == Min.Z) normal = new Vector3(0, 0, -1);
        if (position.Z == Max.Z) normal = new Vector3(0, 0, 1);
        return normal;
    }

    public override Vector2 GetUV(Vector3 position)
    {
        throw new System.NotImplementedException();
    }
}
