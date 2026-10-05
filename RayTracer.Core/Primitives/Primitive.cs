using System.Numerics;

using RayTracer.Core.Materials;
using RayTracer.Core.Math;
using RayTracer.Core.Primitives;

namespace RayTracer.Core;

public abstract class Primitive
{
    public Material Material { get; set; }
    public Texture? Texture { get; set; }

    protected Primitive(Material material, Texture? texture)
    {
        Material = material;
        Texture = texture;
    }

    public abstract PrimitiveType GetPrimitiveType();

    public abstract RayIntersection Intersects(Ray ray, ref float distance);

    // Capture shading data while resolving this ray's intersection.
    public virtual IntersectionResult Intersect(Ray ray, float maxDistance)
    {
        float distance = maxDistance;
        RayIntersection result = Intersects(ray, ref distance);
        Vector3 normal = result == RayIntersection.Miss
            ? Vector3.Zero
            : GetNormal(ray.Origin + ray.Direction * distance);
        return new IntersectionResult(result, distance, normal);
    }

    public abstract Vector3 GetNormal(Vector3 position);

    public abstract Vector2 GetUV(Vector3 position);
}
