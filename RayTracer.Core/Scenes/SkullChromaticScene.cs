using System;
using System.Collections.Generic;
using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

/// <summary>Copper and petrol-blue skulls exchange colored reflections in a neutral studio.</summary>
public sealed class SkullChromaticScene : Scene
{
    public Mesh CopperSkull { get; }
    public Mesh BlueSkull { get; }
    public Mesh CopperBrow { get; }
    public Mesh BlueBrow { get; }

    public SkullChromaticScene()
    {
        Material copper = new(new(.95f, .25f, .055f), .5f, .85f, .065f) { Shininess = 120, Emission = .06f };
        Material blue = new(new(.025f, .36f, .72f), .5f, .85f, .065f) { Shininess = 120, Emission = .06f };
        float yaw = 60 * MathF.PI / 180;
        CopperSkull = SkullEchoScene.Skull(copper, 20.5f, 18.7f, 6, yaw, centerY: .4f);
        BlueSkull = SkullEchoScene.Skull(blue, 20.5f, -18.7f, 6, -yaw, centerY: .4f);
        CopperBrow = PolishBrow(CopperSkull, new(18.7f, .4f, 6), new(-18.7f, .4f, 6));
        BlueBrow = PolishBrow(BlueSkull, new(-18.7f, .4f, 6), new(18.7f, .4f, 6));
        AddObject(CopperSkull);
        AddObject(BlueSkull);
        AddObject(CopperBrow);
        AddObject(BlueBrow);
        Environment = StudioEnvironment;
        AddLighting(new Light(new(-25, 26, -24), float.MinValue, new Material(new(.9f, .87f, .82f))));
        AddLighting(new Light(new(25, 26, -24), float.MinValue, new Material(new(.9f, .87f, .82f))));
        AddLighting(new Light(new(0, 5, -24), float.MinValue, new Material(new(.22f, .22f, .22f))));
        Camera.Target = new(0, 0, 6);
    }

    internal static Mesh PolishBrow(Mesh skull, Vector3 center, Vector3 opposite)
    {
        Vector3 approximateFacet = center + new Vector3(center.X > 0 ? -6 : 6, 6.5f, -4);
        Vector3 normal = Vector3.Normalize(Vector3.Normalize(new Vector3(0, 0, -30) - approximateFacet)
            + Vector3.Normalize(opposite - new Vector3(0, 4, 0) - approximateFacet));
        const float cut = 4.4f;
        // Refine only the brow so the polished boundary follows the sculpted contour cleanly.
        for (int pass = 0; pass < 3; pass++)
        {
            var edges = new Dictionary<(int, int), int>();
            int Midpoint(int a, int b)
            {
                var edge = a < b ? (a, b) : (b, a);
                if (edges.TryGetValue(edge, out int existing)) return existing;
                int index = skull.Vertices.Count;
                skull.Vertices.Add((skull.Vertices[a] + skull.Vertices[b]) / 2);
                Vector3 averagedNormal = skull.VertexNormals[a] + skull.VertexNormals[b];
                skull.VertexNormals.Add(averagedNormal.LengthSquared() > 0 ? Vector3.Normalize(averagedNormal) : Vector3.Zero);
                edges[edge] = index;
                return index;
            }
            bool NearBrow(int index)
            {
                Vector3 v = skull.Vertices[index] - center;
                return v.Y > 2 && Vector3.Dot(v, normal) > cut - 1;
            }
            for (int i = skull.Triangles.Count - 1; i >= 0; i--)
            {
                var (a, b, c) = skull.Triangles[i];
                if (!NearBrow(a) && !NearBrow(b) && !NearBrow(c)) continue;
                int ab = Midpoint(a, b), bc = Midpoint(b, c), ca = Midpoint(c, a);
                skull.Triangles.RemoveAt(i);
                skull.Triangles.Add((a, ab, ca));
                skull.Triangles.Add((ab, b, bc));
                skull.Triangles.Add((ca, bc, c));
                skull.Triangles.Add((ab, bc, ca));
            }
        }
        var polished = new bool[skull.Vertices.Count];
        for (int i = 0; i < skull.Vertices.Count; i++)
        {
            Vector3 relative = skull.Vertices[i] - center;
            float distance = Vector3.Dot(relative, normal) - cut;
            if (relative.Y > 3 && distance > 0)
            {
                skull.Vertices[i] -= normal * distance;
                skull.VertexNormals[i] = normal;
                polished[i] = true;
            }
        }
        // The silver facet is part of the skull surface; triangles are partitioned, never overlaid.
        var facet = new Mesh(new Material(new(.05f), .02f, .98f, .025f) { Shininess = 140 }, null);
        facet.Vertices.AddRange(skull.Vertices);
        facet.VertexNormals.AddRange(skull.VertexNormals);
        for (int i = skull.Triangles.Count - 1; i >= 0; i--)
        {
            var triangle = skull.Triangles[i];
            if (polished[triangle.a] && polished[triangle.b] && polished[triangle.c])
            {
                facet.Triangles.Add(triangle);
                skull.Triangles.RemoveAt(i);
            }
        }
        skull.BuildAcceleration(smoothShading: true);
        facet.BuildAcceleration(smoothShading: true);
        return facet;
    }

    private static Vector3 StudioEnvironment(Vector3 direction)
    {
        Vector3 d = Vector3.Normalize(direction);
        float paper = System.Math.Clamp((d.Z + .35f) / 1.2f, 0, 1);
        Vector3 tone = Vector3.Lerp(new(.055f, .06f, .065f), new(.63f, .605f, .555f), paper);
        tone *= 1 + .06f * d.Y;
        if (d.Z < -.08f)
        {
            float u = d.X / -d.Z, v = d.Y / -d.Z;
            float aperture = MathF.Exp(-MathF.Pow(MathF.Abs(u / 1.35f), 8)
                - MathF.Pow(MathF.Abs((v - 1.1f) / .5f), 8));
            tone = Vector3.Lerp(tone, new(.72f, .71f, .68f), aperture);
        }
        return tone;
    }
}
