using System;
using System.Collections.Generic;
using System.Numerics;
using RayTracer.Core.Math;

namespace RayTracer.Core.Primitives;

/// <summary>Immutable, median-split BVH for a completed mesh. Traversal state belongs to each ray.</summary>
internal sealed class MeshAcceleration
{
    private readonly struct Face
    {
        public readonly Vector3 A, E1, E2, Normal, Min, Max, Center, N0, N1, N2;
        public readonly int Index;
        public readonly bool Smooth;
        public Face(Mesh mesh, int index, bool smooth)
        {
            var tri = mesh.Triangles[index];
            A = mesh.Vertices[tri.a];
            Vector3 b = mesh.Vertices[tri.b], c = mesh.Vertices[tri.c];
            E1 = b - A; E2 = c - A;
            Vector3 cross = Vector3.Cross(E1, E2);
            Normal = cross.LengthSquared() > 0 ? Vector3.Normalize(cross) : Vector3.Zero;
            Min = Vector3.Min(A, Vector3.Min(b, c)) - new Vector3(.0001f);
            Max = Vector3.Max(A, Vector3.Max(b, c)) + new Vector3(.0001f);
            Center = (A + b + c) / 3;
            Index = index;
            Smooth = smooth && mesh.VertexNormals.Count == mesh.Vertices.Count;
            N0 = Smooth ? mesh.VertexNormals[tri.a] : Normal;
            N1 = Smooth ? mesh.VertexNormals[tri.b] : Normal;
            N2 = Smooth ? mesh.VertexNormals[tri.c] : Normal;
        }
    }

    private readonly struct Node(Vector3 min, Vector3 max, int start, int count, int left, int right)
    {
        public readonly Vector3 Min = min, Max = max;
        public readonly int Start = start, Count = count, Left = left, Right = right;
    }

    private readonly Face[] faces;
    private readonly Node[] nodes;

    public MeshAcceleration(Mesh mesh, bool smoothShading)
    {
        faces = new Face[mesh.Triangles.Count];
        for (int i = 0; i < faces.Length; i++) faces[i] = new Face(mesh, i, smoothShading);
        var building = new List<Node>();
        if (faces.Length > 0) Build(building, 0, faces.Length);
        nodes = building.ToArray();
    }

    private int Build(List<Node> building, int start, int count)
    {
        Vector3 min = new(float.MaxValue), max = new(float.MinValue);
        Vector3 centersMin = min, centersMax = max;
        for (int i = start; i < start + count; i++)
        {
            min = Vector3.Min(min, faces[i].Min); max = Vector3.Max(max, faces[i].Max);
            centersMin = Vector3.Min(centersMin, faces[i].Center);
            centersMax = Vector3.Max(centersMax, faces[i].Center);
        }
        int index = building.Count;
        building.Add(default);
        if (count <= 8)
        {
            building[index] = new Node(min, max, start, count, -1, -1);
            return index;
        }
        Vector3 extent = centersMax - centersMin;
        int axis = extent.X >= extent.Y && extent.X >= extent.Z ? 0 : extent.Y >= extent.Z ? 1 : 2;
        Array.Sort(faces, start, count, Comparer<Face>.Create((a, b) =>
        {
            int compared = Component(a.Center, axis).CompareTo(Component(b.Center, axis));
            return compared != 0 ? compared : a.Index.CompareTo(b.Index);
        }));
        int half = count / 2;
        int left = Build(building, start, half), right = Build(building, start + half, count - half);
        building[index] = new Node(min, max, 0, 0, left, right);
        return index;
    }

    private static float Component(Vector3 v, int axis) => axis == 0 ? v.X : axis == 1 ? v.Y : v.Z;

    private static bool Bounds(Node node, Ray ray, float limit, out float near)
    {
        near = 0;
        float far = limit;
        for (int axis = 0; axis < 3; axis++)
        {
            float origin = Component(ray.Origin, axis), direction = Component(ray.Direction, axis);
            float min = Component(node.Min, axis), max = Component(node.Max, axis);
            if (direction == 0)
            {
                if (origin < min || origin > max) return false;
                continue;
            }
            float a = (min - origin) / direction, b = (max - origin) / direction;
            if (a > b) (a, b) = (b, a);
            near = MathF.Max(near, a); far = MathF.Min(far, b);
            if (near > far) return false;
        }
        return far > 0;
    }

    public IntersectionResult Intersect(Ray ray, float maxDistance)
    {
        float closest = maxDistance;
        int closestIndex = int.MaxValue;
        Vector3 normal = Vector3.Zero;
        if (nodes.Length > 0 && maxDistance > 0)
            Visit(0, ray, maxDistance, ref closest, ref closestIndex, ref normal);
        return new(closestIndex == int.MaxValue ? RayIntersection.Miss : RayIntersection.Hit, closest, normal);
    }

    private void Visit(int index, Ray ray, float maxDistance, ref float closest, ref int closestIndex, ref Vector3 normal)
    {
        Node node = nodes[index];
        if (!Bounds(node, ray, closest, out _)) return;
        if (node.Count > 0)
        {
            for (int i = node.Start; i < node.Start + node.Count; i++)
            {
                ref readonly Face face = ref faces[i];
                Vector3 cross = Vector3.Cross(ray.Direction, face.E2);
                float determinant = Vector3.Dot(face.E1, cross);
                if (MathF.Abs(determinant) < 1e-8f) continue;
                float inverse = 1 / determinant;
                Vector3 offset = ray.Origin - face.A;
                float u = Vector3.Dot(offset, cross) * inverse;
                if (u < 0 || u > 1) continue;
                Vector3 q = Vector3.Cross(offset, face.E1);
                float v = Vector3.Dot(ray.Direction, q) * inverse;
                if (v < 0 || u + v > 1) continue;
                float t = Vector3.Dot(face.E2, q) * inverse;
                if (!(t > 1e-6f && t < maxDistance && t <= closest) ||
                    t == closest && face.Index >= closestIndex) continue;
                closest = t; closestIndex = face.Index; normal = face.Normal;
                if (face.Smooth)
                {
                    Vector3 interpolated = face.N0 * (1 - u - v) + face.N1 * u + face.N2 * v;
                    if (interpolated.LengthSquared() > 1e-12f)
                    {
                        normal = Vector3.Normalize(interpolated);
                        if (Vector3.Dot(normal, face.Normal) < 0) normal = -normal;
                    }
                }
            }
            return;
        }
        bool hitLeft = Bounds(nodes[node.Left], ray, closest, out float leftNear);
        bool hitRight = Bounds(nodes[node.Right], ray, closest, out float rightNear);
        if (hitLeft && hitRight)
        {
            int first = leftNear <= rightNear ? node.Left : node.Right;
            int second = leftNear <= rightNear ? node.Right : node.Left;
            Visit(first, ray, maxDistance, ref closest, ref closestIndex, ref normal);
            Visit(second, ray, maxDistance, ref closest, ref closestIndex, ref normal);
        }
        else if (hitLeft) Visit(node.Left, ray, maxDistance, ref closest, ref closestIndex, ref normal);
        else if (hitRight) Visit(node.Right, ray, maxDistance, ref closest, ref closestIndex, ref normal);
    }
}
