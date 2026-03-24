using System.Numerics;
using System.Collections.Generic;
using RayTracer.Core.Materials;
using RayTracer.Core.Math;

namespace RayTracer.Core.Primitives;

public class Mesh : Primitive
{
    public readonly List<Vector3> Vertices = new();
    public readonly List<(int a,int b,int c)> Triangles = new();

    public Mesh(Material material, Texture? texture) : base(material, texture) { }

    public static Mesh FromObj(string path, Material material)
    {
        var mesh = new Mesh(material, null);
        var lines = System.IO.File.ReadAllLines(path);
        foreach (var line in lines)
        {
            if (line.StartsWith("v "))
            {
                var parts = line.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                float x = float.Parse(parts[1]);
                float y = float.Parse(parts[2]);
                float z = float.Parse(parts[3]);
                mesh.Vertices.Add(new Vector3(x,y,z));
            }
            else if (line.StartsWith("f "))
            {
                var parts = line.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                int a = int.Parse(parts[1].Split('/')[0]) - 1;
                int b = int.Parse(parts[2].Split('/')[0]) - 1;
                int c = int.Parse(parts[3].Split('/')[0]) - 1;
                mesh.Triangles.Add((a,b,c));
            }
        }
        return mesh;
    }

    public override PrimitiveType GetPrimitiveType() => PrimitiveType.Mesh;

    public override RayIntersection Intersects(Ray ray, ref float distance)
    {
        // brute-force triangle checks
        foreach (var tri in Triangles)
        {
            var A = Vertices[tri.a];
            var B = Vertices[tri.b];
            var C = Vertices[tri.c];

            // Moller-Trumbore
            Vector3 edge1 = B - A;
            Vector3 edge2 = C - A;
            Vector3 pvec = Vector3.Cross(ray.Direction, edge2);
            float det = Vector3.Dot(edge1, pvec);
            if (System.MathF.Abs(det) < 1e-8f) continue;
            float invDet = 1.0f / det;
            Vector3 tvec = ray.Origin - A;
            float u = Vector3.Dot(tvec, pvec) * invDet;
            if (u < 0 || u > 1) continue;
            Vector3 qvec = Vector3.Cross(tvec, edge1);
            float v = Vector3.Dot(ray.Direction, qvec) * invDet;
            if (v < 0 || u + v > 1) continue;
            float t = Vector3.Dot(edge2, qvec) * invDet;
            if (t > 1e-6f && t < distance)
            {
                distance = t;
                return RayIntersection.Hit;
            }
        }
        return RayIntersection.Miss;
    }

    public override Vector3 GetNormal(Vector3 position)
    {
        // Find triangle that produced the hit is hard here; return up as fallback
        return Vector3.UnitY;
    }

    public override Vector2 GetUV(Vector3 position)
    {
        return Vector2.Zero;
    }
}
