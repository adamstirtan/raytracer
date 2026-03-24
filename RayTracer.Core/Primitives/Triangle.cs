using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Math;

namespace RayTracer.Core.Primitives;

public class Triangle : Primitive
{
    public Vector3 A { get; set; }
    public Vector3 B { get; set; }
    public Vector3 C { get; set; }

    public Triangle(Vector3 a, Vector3 b, Vector3 c, Material material, Texture? texture)
        : base(material, texture)
    {
        A = a;
        B = b;
        C = c;
    }

    public override PrimitiveType GetPrimitiveType() => PrimitiveType.Triangle;

    public override RayIntersection Intersects(Ray ray, ref float distance)
    {
        // Moller-Trumbore
        Vector3 edge1 = B - A;
        Vector3 edge2 = C - A;
        Vector3 pvec = Vector3.Cross(ray.Direction, edge2);
        float det = Vector3.Dot(edge1, pvec);

        if (MathF.Abs(det) < 1e-8f)
            return RayIntersection.Miss;

        float invDet = 1.0f / det;
        Vector3 tvec = ray.Origin - A;
        float u = Vector3.Dot(tvec, pvec) * invDet;
        if (u < 0 || u > 1)
            return RayIntersection.Miss;

        Vector3 qvec = Vector3.Cross(tvec, edge1);
        float v = Vector3.Dot(ray.Direction, qvec) * invDet;
        if (v < 0 || u + v > 1)
            return RayIntersection.Miss;

        float t = Vector3.Dot(edge2, qvec) * invDet;
        if (t > 1e-6f && t < distance)
        {
            distance = t;
            return RayIntersection.Hit;
        }

        return RayIntersection.Miss;
    }

    public override Vector3 GetNormal(Vector3 position)
    {
        var normal = Vector3.Normalize(Vector3.Cross(B - A, C - A));
        return normal;
    }

    public override Vector2 GetUV(Vector3 position)
    {
        // Barycentric coords for UV - simple planar mapping
        Vector3 v0 = B - A;
        Vector3 v1 = C - A;
        Vector3 v2 = position - A;
        float d00 = Vector3.Dot(v0, v0);
        float d01 = Vector3.Dot(v0, v1);
        float d11 = Vector3.Dot(v1, v1);
        float d20 = Vector3.Dot(v2, v0);
        float d21 = Vector3.Dot(v2, v1);
        float denom = d00 * d11 - d01 * d01;
        float v = (d11 * d20 - d01 * d21) / denom;
        float w = (d00 * d21 - d01 * d20) / denom;
        float u = 1.0f - v - w;
        return new Vector2(u, v);
    }
}
