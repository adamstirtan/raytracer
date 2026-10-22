using System;
using System.IO;
using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

/// <summary>Bone and smoked metal above an obsidian floor, connected by a real angled mirror.</summary>
public sealed class SkullEchoScene : Scene
{
    public Mesh BoneSkull { get; }
    public Mesh MetalSkull { get; }

    public SkullEchoScene()
    {
        Material bone = new(new(.76f, .7f, .58f), .85f, .07f, .08f) { Shininess = 80, Emission = .055f };
        Material metal = new(new(.3f, .32f, .34f), .48f, .62f, .14f) { Shininess = 120, Emission = .035f };
        Material slate = new(new(.11f, .115f, .12f), .65f, .15f, .06f) { Shininess = 80 };
        Material brass = new(new(.36f, .27f, .15f), .7f, .24f, .1f) { Shininess = 80 };
        BoneSkull = Skull(bone, 12, 16, 10, .35f);
        MetalSkull = Skull(metal, 11.4f, -19, 9, -.6f);
        AddObject(BoneSkull); AddObject(MetalSkull);

        AddObject(new Primitives.Plane(Vector3.UnitY, 0,
            new Material(new(.065f, .067f, .07f), .4f, .72f, .08f) { Shininess = 110, Emission = .2f }, null));
        AddObject(new Primitives.Plane(-Vector3.UnitZ, 95,
            new Material(new(.075f, .073f, .07f), .6f, 0, 0) { Emission = .55f }, null));
        foreach (var center in new[] { new Vector3(16, 0, 10), new Vector3(-19, 0, 9) })
        {
            AddObject(new Box(center + new Vector3(-5, 0, -4), center + new Vector3(5, 1, 4), slate, null));
            AddObject(new Box(center + new Vector3(-4.8f, 1, -3.8f), center + new Vector3(4.8f, 1.08f, 3.8f), brass, null));
        }

        Vector3 mirrorCenter = new(20, 7.5f, 3);
        Vector3 viewer = new(0, 7, -26), opposite = new(-19, 7, 9);
        Vector3 normal = Vector3.Normalize(Vector3.Normalize(viewer - mirrorCenter) + Vector3.Normalize(opposite - mirrorCenter));
        Quaternion turn = Quaternion.CreateFromAxisAngle(Vector3.Normalize(Vector3.Cross(Vector3.UnitZ, normal)), MathF.Acos(normal.Z));
        AddObject(new RotatedPrimitive(new Box(new(-3.4f, -7, -.15f), new(3.4f, 7, .15f), brass, null), turn, mirrorCenter));
        AddObject(new RotatedPrimitive(new Box(new(-3.2f, -6.8f, .15f), new(3.2f, 6.8f, .18f),
            new Material(new(.15f, .16f, .17f), .015f, .98f, .015f) { Shininess = 120 }, null), turn, mirrorCenter));

        void Panel(Vector3 min, Vector3 max, Vector3 color)
            => AddObject(new Box(min, max, new Material(color) { Emission = 1 }, null));
        Panel(new(60, 8, -40), new(60.15f, 30, 8), new(.85f, .75f, .6f));
        Panel(new(-60.15f, 8, -40), new(-60, 30, 8), new(.55f, .6f, .66f));
        Panel(new(-35, 40, -18), new(35, 40.1f, -8), new(.75f, .77f, .8f));
        Panel(new(-32, 22, -40), new(32, 24, -39.9f), new(.85f, .78f, .66f));
        AddLight(new Light(new(18, 22, -12), float.MinValue, new Material(new(.95f, .8f, .62f))));
        AddLight(new Light(new(-22, 16, -10), float.MinValue, new Material(new(.62f, .66f, .73f))));
        AddLight(new Light(new(0, 22, 28), float.MinValue, new Material(new(.6f, .62f, .65f))));
        Camera.Target = new(0, 5, 10);
    }

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
