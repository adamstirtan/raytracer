using System.Numerics;

using System;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

public class SphereScene : Scene
{
    public SphereScene()
    {
        // Ground plane anchors the scene and provides subtle reflections
        Primitives.Plane ground = new(new Vector3(0, 1, 0), 1000, new Material(new Vector3(0.18f, 0.18f, 0.18f),
            diffuse: 0.95f,
            reflection: 0.15f,
            specular: 0.2f),
            null);

        AddObject(ground);

        // Cluster of reflective spheres (not a grid) — deterministic placement for repeatability
        float centerZ = 10f;
        int count = 14;

        for (int i = 0; i < count; i++)
        {
            float angle = i * (MathF.PI * 2f / count);
            float ring = 1.8f + (i % 3) * 0.9f; // vary radial distance
            float x = MathF.Cos(angle) * ring;
            float z = centerZ + MathF.Sin(angle) * ring;
            float y = 0.4f + ((i * 97) % 5) * 0.18f; // deterministic variation
            float radius = 0.35f + ((i % 4) * 0.2f);

            Vector3 color = new Vector3(
                0.25f + 0.65f * MathF.Abs(MathF.Sin(angle * 1.7f)),
                0.25f + 0.65f * MathF.Abs(MathF.Sin(angle * 2.3f + 1.0f)),
                0.25f + 0.65f * MathF.Abs(MathF.Sin(angle * 2.9f + 2.0f))
            );

            var mat = new Material(color, diffuse: 0.12f, reflection: 0.9f, specular: 0.9f);
            AddObject(new Sphere(new Vector3(x, y, z), radius, mat, null));
        }

        // Add a couple larger reflective anchors for visual interest
        AddObject(new Sphere(new Vector3(0f, 1.6f, centerZ - 0.5f), 1.5f, new Material(new Vector3(0.9f, 0.9f, 1f),
            diffuse: 0.08f, reflection: 0.97f, specular: 1f), null));

        AddObject(new Sphere(new Vector3(-3.0f, 0.9f, centerZ + 1.5f), 1.0f, new Material(new Vector3(0.9f, 0.75f, 0.6f),
            diffuse: 0.08f, reflection: 0.95f, specular: 0.9f), null));

        // Lights
        AddLight(new Light(new Vector3(0f, 12f, -8f), float.MinValue, new Material(new Vector3(1f, 1f, 0.95f))));
        AddLight(new Light(new Vector3(6f, 6f, 8f), float.MinValue, new Material(new Vector3(0.8f, 0.9f, 1f))));

        // Aim camera at the cluster center
        Camera.Target = new Vector3(0f, 0.8f, centerZ);
    }
}