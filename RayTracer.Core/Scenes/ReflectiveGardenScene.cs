using System.Numerics;
using System;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

public class ReflectiveGardenScene : Scene
{
    public ReflectiveGardenScene(int seed = 42, int sphereCount = 300)
    {
        // Ground
        var ground = new Plane(new Vector3(0, 1, 0), 1000, new Material(new Vector3(0.08f, 0.08f, 0.08f), diffuse: 0.9f, reflection: 0.3f, specular: 0.2f), null);
        AddObject(ground);

        // Back wall (large box as far plane)
        var back = new Plane(new Vector3(0, 0, 1), 2000, new Material(new Vector3(0.06f, 0.06f, 0.06f), diffuse: 0.9f, reflection: 0.1f, specular: 0.1f), null);
        AddObject(back);

        // A few large focal reflective spheres
        AddObject(new Sphere(new Vector3(-4, 1, 20), 2.5f, new Material(new Vector3(0.95f, 0.9f, 0.8f), diffuse: 0.3f, reflection: 0.95f, specular: 1.0f), null));
        AddObject(new Sphere(new Vector3(4, 1.2f, 22), 2.8f, new Material(new Vector3(0.8f, 0.9f, 0.95f), diffuse: 0.3f, reflection: 0.9f, specular: 0.9f), null));
        AddObject(new Sphere(new Vector3(0, 2.2f, 18), 1.8f, new Material(new Vector3(0.95f, 0.7f, 0.85f), diffuse: 0.2f, reflection: 0.97f, specular: 1.0f), null));

        // Randomized small spheres (the 'garden')
        var rnd = new Random(seed);
        for (int i = 0; i < sphereCount; i++)
        {
            float angle = (float)(rnd.NextDouble() * Math.PI * 2.0);
            float radius = (float)(5.0 + rnd.NextDouble() * 12.0); // ring
            float x = MathF.Cos(angle) * radius + (float)(rnd.NextDouble() - 0.5) * 0.6f;
            float z = MathF.Sin(angle) * radius + (float)(rnd.NextDouble() - 0.5) * 0.6f + 15.0f;
            float y = (float)(rnd.NextDouble() * 1.6);
            float size = (float)(0.15 + rnd.NextDouble() * 0.6);

            // Material: mix metallic and glossy
            var hue = (float)rnd.NextDouble();
            var color = HsvToRgb(hue, 0.6f + (float)rnd.NextDouble() * 0.4f, 0.6f + (float)rnd.NextDouble() * 0.4f);
            float reflection = (float)(0.2 + rnd.NextDouble() * 0.9);
            float diffuse = (float)(0.2 + rnd.NextDouble() * 0.8);
            float specular = (float)(0.1 + rnd.NextDouble() * 0.9);

            var mat = new Material(new Vector3(color.X, color.Y, color.Z), diffuse: diffuse, reflection: reflection, specular: specular);
            var s = new Sphere(new Vector3(x, y + size, z), size, mat, null);
            AddObject(s);
        }

        // Mirrors: some large thin boxes acting as mirrors
        AddObject(new Box(new Vector3(-12, 2, 18), new Vector3(-11.5f, 8, 26), new Material(new Vector3(0.95f,0.95f,0.95f), diffuse: 0.05f, reflection: 0.98f, specular: 1.0f), null));
        AddObject(new Box(new Vector3(11.5f, 2, 20), new Vector3(12f, 8, 30), new Material(new Vector3(0.95f,0.95f,0.9f), diffuse: 0.05f, reflection: 0.98f, specular: 1.0f), null));

        // Lights
        AddLight(new Light(new Vector3(0, 40, -60), float.MinValue, new Material(new Vector3(1.0f, 0.98f, 0.9f))));
        AddLight(new Light(new Vector3(-10, 10, 10), float.MinValue, new Material(new Vector3(0.7f, 0.8f, 1.0f))));
        AddLight(new Light(new Vector3(10, 8, 8), float.MinValue, new Material(new Vector3(1.0f, 0.8f, 0.7f))));

        // Camera defaults can be overridden by RenderOptions
    }

    private static Vector3 HsvToRgb(float h, float s, float v)
    {
        // h in 0..1
        int i = (int)MathF.Floor(h * 6);
        float f = h * 6 - i;
        float p = v * (1 - s);
        float q = v * (1 - f * s);
        float t = v * (1 - (1 - f) * s);
        float r=0,g=0,b=0;
        switch (i % 6)
        {
            case 0: r = v; g = t; b = p; break;
            case 1: r = q; g = v; b = p; break;
            case 2: r = p; g = v; b = t; break;
            case 3: r = p; g = q; b = v; break;
            case 4: r = t; g = p; b = v; break;
            case 5: r = v; g = p; b = q; break;
        }
        return new Vector3(r,g,b);
    }
}
