using System;
using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

public class ObservatoryScene : Scene
{
    public ObservatoryScene()
    {
        Material gold = new(new Vector3(0.95f, 0.57f, 0.16f), 0.8f, 0.3f, 0.1f);
        Material stone = new(new Vector3(0.2f, 0.28f, 0.38f), 0.8f, 0.15f, 0.03f);
        Material pale = new(new Vector3(0.6f, 0.68f, 0.76f), 0.85f, 0.12f, 0.03f);
        Material blue = new(new Vector3(0.03f, 0.25f, 0.55f), 0.9f, 0.45f, 0.08f);
        Material mountain = new(new Vector3(0.13f, 0.18f, 0.3f), 0.8f, 0, 0);
        AddObject(new Primitives.Plane(Vector3.UnitY, 0,
            new Material(new Vector3(0.025f, 0.055f, 0.12f), 0.35f, 0.8f, 0.02f), null));

        void Block(Vector3 min, Vector3 max, Material material) => AddObject(new Box(min, max, material, null));
        void Column(Vector3 center, float radius, float height, Material material)
            => AddObject(new Cylinder(center, radius, height, material, null));

        // Terraces and a narrow processional bridge above the lake.
        Block(new(-9, 0.1f, 3), new(9, 0.6f, 20), stone);
        Block(new(-7.5f, 0.6f, 5), new(7.5f, 1.1f, 18), pale);
        Block(new(-6, 1.1f, 7), new(6, 1.5f, 17), stone);
        Block(new(-2.2f, 0.15f, -12), new(2.2f, 0.45f, 3), stone);
        for (int i = 0; i < 6; i++)
            Block(new(-3 - i * 0.35f, 0.45f, 3 + i * 0.65f), new(3 + i * 0.35f, 0.55f + i * 0.15f, 3.65f + i * 0.65f), pale);

        Vector3 focus = new(0, 8, 12);
        Column(new(0, 2.3f, 12), 2.4f, 1.6f, gold);
        Column(new(0, 3.2f, 12), 1.5f, 0.3f, stone);
        // Three differently tilted rings form the armillary sculpture.
        AddObject(new RotatedPrimitive(new Torus(gold, null, 4.6f, 0.22f),
            Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2), focus));
        AddObject(new RotatedPrimitive(new Torus(gold, null, 5.1f, 0.18f),
            Quaternion.CreateFromYawPitchRoll(0.65f, 0.75f, 0.4f), focus));
        AddObject(new RotatedPrimitive(new Torus(pale, null, 4.1f, 0.12f),
            Quaternion.CreateFromYawPitchRoll(-0.5f, 1.15f, -0.55f), focus));
        AddLight(new Light(focus, 1.25f, new Material(new Vector3(0.04f, 0.2f, 0.45f))));

        for (int i = 0; i < 8; i++)
        {
            float angle = i * MathF.Tau / 8;
            Vector3 position = new(MathF.Cos(angle) * 6.4f, 3.5f, 12 + MathF.Sin(angle) * 4.7f);
            Column(position, 0.18f, 4, gold);
            AddObject(new Sphere(position + Vector3.UnitY * 2.2f, 0.42f, blue, null));
            AddLight(new Light(position + Vector3.UnitY * 2.8f, 0.1f, new Material(new Vector3(0.001f, 0.003f, 0.006f))));
        }
        foreach (float x in new[] { -8.5f, 8.5f })
        {
            Column(new(x, 3.7f, 10), 0.5f, 6.2f, stone);
            Column(new(x, 6.85f, 10), 0.65f, 0.2f, gold);
            AddObject(new Sphere(new(x, 8.1f, 10), 0.85f, gold, null));
        }
        for (int i = 0; i < 8; i++)
        {
            float z = -10 + i * 2;
            foreach (float x in new[] { -2f, 2f })
            {
                Column(new(x, 0.75f, z), 0.09f, 0.6f, gold);
                AddLight(new Light(new(x, 1.1f, z), 0.12f, new Material(new Vector3(0.003f, 0.006f, 0.012f))));
            }
        }

        var random = new Random(9);
        for (int i = 0; i < 16; i++)
        {
            float x = -65 + i * 8;
            float z = 46 + (float)random.NextDouble() * 12;
            Vector3 a = new(x - 7, 0, z);
            Vector3 b = new(x + 9, 0, z);
            Vector3 peak = new(x + 1, 9 + (float)random.NextDouble() * 11, z + 5);
            Vector3 back = new(x + 3, 0, z + 17);
            AddObject(new Triangle(a, peak, b, mountain, null));
            AddObject(new Triangle(b, peak, back, stone, null));
            AddObject(new Triangle(back, peak, a, mountain, null));
        }
        AddObject(new Sphere(new(-25, 22, 70), 10,
            new Material(new Vector3(0.3f, 0.42f, 0.65f), 0.9f, 0, 0), null));
        AddLight(new Light(new(-20, 26, -8), float.MinValue, new Material(new Vector3(1.35f, 0.95f, 0.55f))));
        AddLight(new Light(new(24, 18, 5), float.MinValue, new Material(new Vector3(0.3f, 0.55f, 0.95f))));
        Camera.Target = new Vector3(0, 5, 12);
    }
}
