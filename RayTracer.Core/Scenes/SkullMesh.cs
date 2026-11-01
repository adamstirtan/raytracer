using System;
using System.IO;
using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

/// <summary>
/// Shared skull-mesh placement helper, formerly SkullEchoScene.Skull.
/// SkullEchoScene itself is now a JSON scene, but skull-chromatic still
/// builds its skulls in code (it subdivides the brow mesh), so the helper
/// lives here instead of on a deleted scene class.
/// </summary>
internal static class SkullMesh
{
    internal static Mesh Skull(Material material, float height, float x, float z, float yaw, float? centerY = null)
    {
        var mesh = Mesh.FromObj(Path.Combine(AppContext.BaseDirectory, "assets", "skull.obj"), material, normalize: false);
        Quaternion rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, yaw + MathF.PI) *
            Quaternion.CreateFromAxisAngle(Vector3.UnitX, -MathF.PI / 2);
        Vector3 min = new(float.MaxValue), max = new(float.MinValue);
        for (int i = 0; i < mesh.Vertices.Count; i++)
        {
            mesh.Vertices[i] = Vector3.Transform(mesh.Vertices[i], rotation);
            min = Vector3.Min(min, mesh.Vertices[i]); max = Vector3.Max(max, mesh.Vertices[i]);
        }
        float scale = height / (max.Y - min.Y);
        Vector3 center = (min + max) / 2;
        Vector3 placement = new(x, centerY ?? height / 2 + 1.1f, z);
        for (int i = 0; i < mesh.Vertices.Count; i++) mesh.Vertices[i] = (mesh.Vertices[i] - center) * scale + placement;
        for (int i = 0; i < mesh.VertexNormals.Count; i++)
        {
            Vector3 n = Vector3.Transform(mesh.VertexNormals[i], rotation);
            mesh.VertexNormals[i] = n.LengthSquared() > 0 ? Vector3.Normalize(n) : Vector3.Zero;
        }
        mesh.BuildAcceleration(smoothShading: true);
        return mesh;
    }
}
