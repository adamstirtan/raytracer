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

    public abstract Vector3 GetNormal(Vector3 position);

    public abstract Vector2 GetUV(Vector3 position);
}