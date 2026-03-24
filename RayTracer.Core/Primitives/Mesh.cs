using System.Numerics;
using System.Collections.Generic;
using RayTracer.Core.Materials;
using RayTracer.Core.Math;

namespace RayTracer.Core.Primitives;

public class Mesh : Primitive
{
    public readonly List<Vector3> Vertices = new();
    public readonly List<(int a,int b,int c)> Triangles = new();
    public readonly List<Vector3> VertexNormals = new();

    // last hit normal captured during Intersects for GetNormal()
    private Vector3 _lastHitNormal = Vector3.UnitY;

    public Mesh(Material material, Texture? texture) : base(material, texture) { }

    public static Mesh FromObj(string path, Material material)
    {
        var mesh = new Mesh(material, null);
        var lines = System.IO.File.ReadAllLines(path);
        var normals = new List<Vector3>();
        var tempNormalsPerVertex = new List<Vector3>();

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
            else if (line.StartsWith("vn "))
            {
                var parts = line.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                float x = float.Parse(parts[1]);
                float y = float.Parse(parts[2]);
                float z = float.Parse(parts[3]);
                normals.Add(new Vector3(x,y,z));
            }
            else if (line.StartsWith("f "))
            {
                var parts = line.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                // face entries may be v, v/vt, v//vn, or v/vt/vn
                int[] vidx = new int[3];
                int[] nidx = new int[3];
                for (int i = 0; i < 3; i++)
                {
                    var comps = parts[1+i].Split('/');
                    vidx[i] = int.Parse(comps[0]) - 1;
                    if (comps.Length >= 3 && comps[2] != "") nidx[i] = int.Parse(comps[2]) - 1;
                    else nidx[i] = -1;
                }
                mesh.Triangles.Add((vidx[0], vidx[1], vidx[2]));

                // store per-triangle vertex normal indices if present
                if (nidx[0] >= 0 && normals.Count > 0)
                {
                    // ensure VertexNormals length matches Vertices
                    while (mesh.VertexNormals.Count < mesh.Vertices.Count) mesh.VertexNormals.Add(Vector3.Zero);
                    for (int i = 0; i < 3; i++)
                    {
                        if (nidx[i] >= 0 && nidx[i] < normals.Count)
                        {
                            mesh.VertexNormals[vidx[i]] = normals[nidx[i]];
                        }
                    }
                }
            }
        }

        // If no vertex normals were supplied, compute per-vertex normals by averaging face normals
        if (mesh.VertexNormals.Count == 0)
        {
            mesh.VertexNormals.AddRange(new Vector3[mesh.Vertices.Count]);
            var counts = new int[mesh.Vertices.Count];
            foreach (var tri in mesh.Triangles)
            {
                var A = mesh.Vertices[tri.a];
                var B = mesh.Vertices[tri.b];
                var C = mesh.Vertices[tri.c];
                var n = Vector3.Normalize(Vector3.Cross(B - A, C - A));
                mesh.VertexNormals[tri.a] += n; counts[tri.a]++;
                mesh.VertexNormals[tri.b] += n; counts[tri.b]++;
                mesh.VertexNormals[tri.c] += n; counts[tri.c]++;
            }
            for (int i = 0; i < mesh.VertexNormals.Count; i++)
            {
                if (counts[i] > 0) mesh.VertexNormals[i] = Vector3.Normalize(mesh.VertexNormals[i] / counts[i]);
                else mesh.VertexNormals[i] = Vector3.UnitY;
            }
        }

        // Compute bounding box and normalize/center the mesh so it fits in a unit cube centered at origin
        if (mesh.Vertices.Count > 0)
        {
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (var v in mesh.Vertices)
            {
                min = Vector3.Min(min, v);
                max = Vector3.Max(max, v);
            }
            var center = (min + max) * 0.5f;
            var extent = max - min;
            float maxExtent = System.Math.Max(extent.X, System.Math.Max(extent.Y, extent.Z));
            float scale = 4.0f / maxExtent; // scale so model ~4 units across
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                mesh.Vertices[i] = (mesh.Vertices[i] - center) * scale + new Vector3(0, 1.0f, 8.0f);
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
                // compute triangle normal and store
                _lastHitNormal = Vector3.Normalize(Vector3.Cross(edge1, edge2));
                return RayIntersection.Hit;
            }
        }
        return RayIntersection.Miss;
    }

    public override Vector3 GetNormal(Vector3 position)
    {
        // return last computed triangle normal
        return _lastHitNormal;
    }

    public override Vector2 GetUV(Vector3 position)
    {
        return Vector2.Zero;
    }
}
