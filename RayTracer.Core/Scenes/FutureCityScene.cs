using System;
using System.Numerics;
using System.IO;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

/// <summary>A terraced spaceport with a ring reactor, skyline, and angular desert terrain.</summary>
public class FutureCityScene : Scene
{
    public FutureCityScene()
    {
        string directory = Path.Combine(AppContext.BaseDirectory, "Textures", "FutureCity");
        Texture Load(string name) => new(Path.Combine(directory, name + ".png"));
        var cladding = Load("alloy");
        var ceramic = Load("ceramic");
        var darkPanels = Load("dark");
        var goldPanels = Load("gold");
        var windows = Load("windows");
        var paving = Load("paving");
        var dome = Load("dome");
        var rock = Load("rock");
        Material alloy = new(new Vector3(0.22f, 0.3f, 0.42f), 0.8f, 0.18f, 0.06f);
        Material pale = new(new Vector3(0.62f, 0.72f, 0.78f), 0.85f, 0.12f, 0.04f);
        Material dark = new(new Vector3(0.045f, 0.075f, 0.12f), 0.8f, 0.25f, 0.03f);
        Material cyan = new(new Vector3(0.04f, 0.85f, 0.92f), 1f, 0.2f, 0.08f);
        Material gold = new(new Vector3(0.95f, 0.46f, 0.12f), 0.9f, 0.2f, 0.06f);
        Material glass = new(new Vector3(0.12f, 0.28f, 0.46f), 0.65f, 0.55f, 0.12f);
        Material terrain = new(new Vector3(0.22f, 0.14f, 0.2f), 0.9f, 0f, 0f);

        cyan.Emission = .35f;
        glass.Emission = .08f;
        foreach (Material m in new[] { alloy, pale, dark, cyan, gold, glass }) m.Shininess = 48;
        Texture? Surface(Material material) => ReferenceEquals(material, alloy) ? cladding :
            ReferenceEquals(material, pale) ? ceramic : ReferenceEquals(material, dark) ? darkPanels :
            ReferenceEquals(material, gold) ? goldPanels : ReferenceEquals(material, glass) ? windows : null;

        AddObject(new Ground(paving));
        AddObject(new Sky(Load("sky")));

        void Block(Vector3 min, Vector3 max, Material material, Texture? texture = null)
            => AddObject(new FacadeBox(min, max, material, texture ?? Surface(material)));
        void Column(float x, float z, float radius, float height, Material material, float bottom = 0)
            => AddObject(new Cylinder(new Vector3(x, bottom + height / 2, z), radius, height, material, Surface(material)));
        void Ring(float x, float y, float z, float radius, float thickness, Material material)
            => AddObject(new Torus(material, Surface(material), radius, thickness, new Vector3(x, y, z)));

        // Broad plaza, stepped approach, and thin cyan guideways.
        Block(new Vector3(-16, 0, -4), new Vector3(16, 0.4f, 30), dark, paving);
        Block(new Vector3(-10, 0.4f, 0), new Vector3(10, 0.7f, 24), alloy, paving);
        for (int i = 0; i < 5; i++)
            Block(new Vector3(-5 - i * 0.4f, 0, -8 + i), new Vector3(5 + i * 0.4f, 0.12f * (i + 1), -7 + i), pale);
        foreach (float x in new[] { -7f, 7f })
        {
            Block(new Vector3(x - 0.14f, 0.71f, -1), new Vector3(x + 0.14f, 0.76f, 24), cyan);
            for (int i = 0; i < 7; i++)
                Block(new Vector3(x - 0.4f, 0.4f, i * 4), new Vector3(x + 0.4f, 0.7f, i * 4 + 0.6f), gold);
        }

        // Central reactor: a dark core, stacked collars, dome, and floating halo.
        Column(0, 12, 5.2f, 0.8f, pale, 0.7f);
        Column(0, 12, 4.3f, 0.5f, dark, 1.5f);
        Column(0, 12, 2.7f, 7.5f, alloy, 2);
        for (int i = 0; i < 5; i++)
        {
            Column(0, 12, 2.85f, 0.16f, cyan, 2.6f + i * 1.35f);
            Column(0, 12, 3.1f, 0.24f, pale, 2.8f + i * 1.35f);
        }
        AddObject(new Sphere(new Vector3(0, 9.3f, 12), 2.65f, glass, dome));
        Ring(0, 11.7f, 12, 4.8f, 0.24f, gold);
        Ring(0, 11.7f, 12, 5.4f, 0.08f, cyan);
        Column(0, 12, 0.2f, 5, gold, 9.5f);
        AddObject(new Sphere(new Vector3(0, 14.5f, 12), 0.48f, cyan, null));
        for (int i = 0; i < 8; i++)
        {
            float angle = i * MathF.PI / 4;
            float x = MathF.Cos(angle) * 4;
            float z = 12 + MathF.Sin(angle) * 4;
            Column(x, z, 0.28f, 2.7f, gold, 1.4f);
        }

        // Asymmetric skyline: setbacks, glass frontages, window bands, and rooftop antennas.
        var random = new Random(42);
        foreach (float x in new[] { -13f, 13f })
        {
            for (int i = 0; i < 5; i++)
            {
                float z = 5 + i * 5.2f;
                float height = 5 + (float)random.NextDouble() * 8;
                float width = 1.3f + (float)random.NextDouble() * 0.5f;
                Block(new Vector3(x - width, 0.4f, z - 1.4f), new Vector3(x + width, height, z + 1.4f), alloy);
                Block(new Vector3(x - width * 0.7f, height, z - 1), new Vector3(x + width * 0.7f, height + 1.2f, z + 1), pale);
                Block(new Vector3(x - width + 0.12f, 1, z - 1.46f), new Vector3(x + width - 0.12f, height - 0.4f, z - 1.41f), glass);
                for (int band = 1; band < height; band += 2)
                    Block(new Vector3(x - width - 0.03f, band, z - 1.48f), new Vector3(x + width + 0.03f, band + 0.12f, z + 1.48f), i % 2 == 0 ? cyan : gold);
                Column(x, z, 0.08f, 1.8f, gold, height + 1.2f);
            }
        }

        // Rounded landing pads and habitation domes in the foreground.
        foreach (float x in new[] { -12f, 12f })
        {
            Column(x, -3, 3.2f, 0.65f, pale);
            Column(x, -3, 2.8f, 0.15f, dark, 0.65f);
            AddObject(new Disk(new Vector3(x, 0.81f, -3), Vector3.UnitY, 2.4f, glass, dome));
            Ring(x, 0.92f, -3, 2.55f, 0.08f, cyan);
            AddObject(new Sphere(new Vector3(x, 0.7f, -3), 1.7f, glass, dome));
            Block(new Vector3(x - 0.8f, 0.8f, -5.2f), new Vector3(x + 0.8f, 1.7f, -4.3f), pale);
        }

        // Elevated bridge and pylons behind the reactor.
        Block(new Vector3(-15, 5.5f, 22), new Vector3(15, 6.1f, 24), pale);
        Block(new Vector3(-15, 6.12f, 22), new Vector3(15, 6.24f, 22.15f), cyan);
        foreach (float x in new[] { -9f, -5f, 5f, 9f }) Column(x, 23, 0.5f, 5.5f, alloy);

        // Triangular mountains form an angular horizon behind the city.
        for (int i = 0; i < 13; i++)
        {
            float x = -42 + i * 7;
            float z = 39 + (float)random.NextDouble() * 6;
            float height = 7 + (float)random.NextDouble() * 10;
            Vector3 peak = new(x + 2, height, z + 4);
            Vector3 left = new(x - 6, 0, z);
            Vector3 right = new(x + 7, 0, z);
            Vector3 back = new(x, 0, z + 12);
            AddObject(new Triangle(left, peak, right, terrain, rock));
            AddObject(new Triangle(right, peak, back, terrain, rock));
            AddObject(new Triangle(back, peak, left, terrain, rock));
        }

        // A distant luminous planet, plus cool key and warm rim lighting.
        AddObject(new Sphere(new Vector3(-24, 27, 62), 6,
            new Material(Vector3.One) { Emission = .8f }, Load("planet")));
        AddLight(new Light(new Vector3(-15, 25, -15), float.MinValue, new Material(new Vector3(1f, 1.1f, 1.2f))));
        AddLight(new Light(new Vector3(20, 18, 20), float.MinValue, new Material(new Vector3(.75f, .44f, .28f))));
        Camera.Target = new Vector3(0, 5, 12);
    }

    // Consistent world-sized panels avoid stretching a whole facade onto one tall wall.
    private sealed class FacadeBox(Vector3 min, Vector3 max, Material material, Texture? texture)
        : Box(min, max, material, texture)
    {
        public override Vector2 GetUV(Vector3 position)
        {
            Vector3 p = position - Min;
            Vector3 normal = GetNormal(position);
            const float repeat = 4;
            if (MathF.Abs(normal.Y) > .5f) return new Vector2(p.X, p.Z) / repeat;
            if (MathF.Abs(normal.X) > .5f) return new Vector2(p.Z, -p.Y) / repeat;
            return new Vector2(p.X, -p.Y) / repeat;
        }
    }

    private sealed class Ground(Texture texture) : Primitives.Plane(Vector3.UnitY, 0,
        new Material(Vector3.One, .8f, .22f, .04f) { Shininess = 64 }, texture)
    {
        public override Vector2 GetUV(Vector3 position) => new Vector2(position.X, position.Z) / 8;
    }

    private sealed class Sky(Texture texture) : Primitives.Plane(-Vector3.UnitZ, 105,
        new Material(Vector3.One) { Emission = 1 }, texture)
    {
        public override Vector2 GetUV(Vector3 position)
            => new((position.X + 150) / 300, System.Math.Clamp((70 - position.Y) / 100, .001f, .999f));
    }
}
