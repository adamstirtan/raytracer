using System.Numerics;

using RayTracer.Core.Materials;
using RayTracer.Core.Math;

namespace RayTracer.Core.Primitives;

public class Triangle : Primitive
{
    public Vector3 Vertex1 { get; set; }
    public Vector3 Vertex2 { get; set; }
    public Vector3 Vertex3 { get; set; }

    public Triangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Material material, Texture texture)
        : base(material, texture)
    {
        Vertex1 = vertex1;
        Vertex2 = vertex2;
        Vertex3 = vertex3;
    }

    public override PrimitiveType GetPrimitiveType()
    {
        return PrimitiveType.Triangle;
    }

    public override RayIntersection Intersects(Ray ray, ref float distance)
    {
        // Möller–Trumbore intersection algorithm
        Vector3 edge1 = Vertex2 - Vertex1;
        Vector3 edge2 = Vertex3 - Vertex1;
        Vector3 h = Vector3.Cross(ray.Direction, edge2);
        float a = Vector3.Dot(edge1, h);

        if (a > -0.0001f && a < 0.0001f)
            return RayIntersection.Miss;

        float f = 1.0f / a;
        Vector3 s = ray.Origin - Vertex1;
        float u = f * Vector3.Dot(s, h);

        if (u < 0.0f || u > 1.0f)
            return RayIntersection.Miss;

        Vector3 q = Vector3.Cross(s, edge1);
        float v = f * Vector3.Dot(ray.Direction, q);

        if (v < 0.0f || u + v > 1.0f)
            return RayIntersection.Miss;

        float t = f * Vector3.Dot(edge2, q);

        if (t > 0.0001f)
        {
            distance = t;
            return RayIntersection.Hit;
        }

        return RayIntersection.Miss;
    }

    public override Vector3 GetNormal(Vector3 position)
    {
        Vector3 edge1 = Vertex2 - Vertex1;
        Vector3 edge2 = Vertex3 - Vertex1;
        return Vector3.Normalize(Vector3.Cross(edge1, edge2));
    }

    public override Vector2 GetUV(Vector3 position)
    {
        return Vector2.Zero;
    }
}
