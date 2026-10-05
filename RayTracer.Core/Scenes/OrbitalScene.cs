using System;
using System.Numerics;
using System.Linq;
using System.IO;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

public class OrbitalScene : Scene
{
    public OrbitalScene()
    {
        string directory = Path.Combine(AppContext.BaseDirectory, "Textures", "Orbital");
        Texture Load(string name) => new(Path.Combine(directory, name + ".png"));
        var hull = Load("hull");
        var blueHull = Load("blue-hull");
        var darkHull = Load("dark-hull");
        var goldHull = Load("gold-hull");
        var solar = Load("solar-cells");
        var rockTexture = Load("rock");
        Material silver = new(new Vector3(0.65f, 0.72f, 0.8f), 0.8f, 0.3f, 0.08f);
        Material dark = new(new Vector3(0.07f, 0.1f, 0.18f), 0.8f, 0.3f, 0.04f);
        Material blue = new(new Vector3(0.045f, 0.2f, 0.55f), 0.9f, 0.25f, 0.06f);
        Material gold = new(new Vector3(0.9f, 0.52f, 0.17f), 0.9f, 0.22f, 0.06f);
        Material cyan = new(new Vector3(0.06f, 0.8f, 1f), 1f, 0.18f, 0.06f);
        Texture? Surface(Material m) => ReferenceEquals(m, silver) ? hull :
            ReferenceEquals(m, blue) ? blueHull : ReferenceEquals(m, dark) ? darkHull :
            ReferenceEquals(m, gold) ? goldHull : null;
        foreach (Material m in new[] { silver, dark, blue, gold, cyan }) m.Shininess = 48;
        void Box(Vector3 min, Vector3 max, Material m, Texture? texture = null)
            => AddObject(new Box(min, max, m, texture ?? Surface(m)));
        void Cylinder(Vector3 p, float r, float h, Material m) => AddObject(new Cylinder(p, r, h, m, Surface(m)));
        void Ring(Vector3 p, float r, float thickness, Material m) => AddObject(new Torus(m, Surface(m), r, thickness, p));

        // Banded gas giant surrounded by a broad, irregular asteroid belt.
        Vector3 planet = new(-15, -13, 43);
        AddObject(new Sphere(planet, 14,
            new Material(Vector3.One, 1f, 0, 0), Load("gas-giant")));
        var random = new Random(73);
        for (int i = 0; i < 180; i++)
        {
            float angle = i * MathF.Tau / 180 + (float)random.NextDouble() * 0.025f;
            float radius = 19 + (float)random.NextDouble() * 7;
            Vector3 center = planet + new Vector3(MathF.Cos(angle) * radius,
                ((float)random.NextDouble() - 0.5f) * 2, MathF.Sin(angle) * radius);
            float size = 0.22f + (float)random.NextDouble() * 0.55f;
            var rock = new Asteroid(center, size, new Material(Vector3.One, 0.9f, 0, 0), rockTexture);
            rock.Vertices.AddRange(new[] {
                center + new Vector3(size * 1.3f, 0, 0), center - new Vector3(size, 0, 0),
                center + new Vector3(0, size * 0.8f, 0), center - new Vector3(0, size, 0),
                center + new Vector3(0, 0, size), center - new Vector3(0, 0, size * 1.2f)
            });
            rock.Triangles.AddRange(new[] { (0,2,4), (4,2,1), (1,2,5), (5,2,0),
                (4,3,0), (1,3,4), (5,3,1), (0,3,5) });
            AddObject(rock);
        }
        AddObject(new Sphere(new Vector3(20, 12, 48), 3.4f, new Material(Vector3.One, .9f, 0, 0), Load("moon")));

        int environmentCount = this.OfType<Primitive>().Count();

        // Twin habitation rings around a vertically stacked command hub.
        Vector3 hub = new(2, 2, 5);
        Cylinder(hub, 1.5f, 7.5f, silver);
        Cylinder(hub, 1.65f, 0.3f, cyan);
        AddObject(new Sphere(hub + Vector3.UnitY * 3.6f, 1.5f, blue, blueHull));
        Ring(hub, 6.2f, 0.7f, silver);
        Ring(hub + Vector3.UnitY * 0.55f, 6.3f, 0.09f, cyan);
        Ring(hub - Vector3.UnitY * 2.1f, 4.5f, 0.45f, gold);
        for (int i = 0; i < 12; i++)
        {
            float a = i * MathF.PI / 6;
            Vector3 p = hub + new Vector3(MathF.Cos(a) * 6.2f, 0, MathF.Sin(a) * 6.2f);
            AddObject(new Sphere(p, 0.85f, i % 3 == 0 ? blue : silver, i % 3 == 0 ? blueHull : hull));
            Cylinder(p + Vector3.UnitY * 0.7f, 0.4f, 0.5f, dark);
        }
        Box(hub + new Vector3(-6.2f,-0.2f,-0.25f), hub + new Vector3(6.2f,0.2f,0.25f), silver);
        Box(hub + new Vector3(-0.25f,-0.2f,-6.2f), hub + new Vector3(0.25f,0.2f,6.2f), silver);

        // Four banks of photovoltaic panels with gold grid lines.
        foreach (float side in new[] { -1f, 1f })
        {
            float x = hub.X + side * 10;
            Box(new Vector3(MathF.Min(hub.X,x),1.8f,4.8f),new Vector3(MathF.Max(hub.X,x),2.2f,5.2f),silver);
            foreach (float z in new[] { -1f, 8f })
            {
                Box(new Vector3(x-2.5f,1.9f,z-3),new Vector3(x+2.5f,2.05f,z+3),blue,solar);
                for (int j=0;j<=5;j++)
                    Box(new Vector3(x-2.5f+j,2.06f,z-3),new Vector3(x-2.47f+j,2.09f,z+3),gold);
                for (int j=0;j<=6;j++)
                    Box(new Vector3(x-2.5f,2.06f,z-3+j),new Vector3(x+2.5f,2.09f,z-2.97f+j),gold);
            }
        }

        // Docking vessel with a pointed nose, swept wings, and engine bells.
        Vector3 ship = new(12, 5, -5);
        Box(ship + new Vector3(-0.8f,-0.4f,-2),ship + new Vector3(0.8f,0.4f,2.5f),silver);
        AddObject(new Sphere(ship + new Vector3(0,0.4f,-0.4f),0.65f,blue,blueHull));
        AddObject(new Triangle(ship+new Vector3(-0.8f,0,-2),ship+new Vector3(0,0,-4),ship+new Vector3(0.8f,0,-2),silver,hull));
        foreach(float side in new[]{-1f,1f})
        {
            AddObject(new Triangle(ship+new Vector3(side*.6f,0,-1),ship+new Vector3(side*3.8f,0,2.5f),ship+new Vector3(side*.6f,0,2),silver,hull));
            AddObject(new Sphere(ship+new Vector3(side*.55f,0,2.5f),.38f,dark,darkHull));
            AddObject(new Sphere(ship+new Vector3(side*.55f,0,2.85f),.22f,cyan,null));
        }

        // Shift the entire station assembly and vessel together, clear of the planet.
        Vector3 stationOffset = new(20, 0, 0);
        foreach (Primitive primitive in this.OfType<Primitive>().Skip(environmentCount))
        {
            switch (primitive)
            {
                case Box box: box.Min += stationOffset; box.Max += stationOffset; break;
                case Cylinder cylinder: cylinder.Center += stationOffset; break;
                case Sphere sphere: sphere.Center += stationOffset; break;
                case Torus torus: torus.Center += stationOffset; break;
                case Triangle triangle:
                    triangle.A += stationOffset; triangle.B += stationOffset; triangle.C += stationOffset;
                    break;
            }
        }

        AddObject(new StarBackdrop(Load("starfield")));
        AddLight(new Light(new Vector3(-25,35,-25),float.MinValue,new Material(new Vector3(1.2f,1.08f,.9f))));
        AddLight(new Light(new Vector3(25,15,-10),float.MinValue,new Material(new Vector3(.25f,.38f,.65f))));
        Camera.Target = new Vector3(3, -1, 15);
    }

    // Spherical projection avoids shared mesh hit state and keeps each rock's map local.
    private sealed class Asteroid(Vector3 center, float size, Material material, Texture texture)
        : Mesh(material, texture)
    {
        public override Vector2 GetUV(Vector3 position)
        {
            Vector3 p = Vector3.Normalize((position - center) / size);
            return new Vector2(.5f + MathF.Atan2(p.Z, p.X) / MathF.Tau,
                .5f - MathF.Asin(p.Y) / MathF.PI);
        }
    }

    private sealed class StarBackdrop(Texture texture) : Primitives.Plane(-Vector3.UnitZ, 90,
        new Material(Vector3.One, 0, 0, 0) { Emission = 1 }, texture)
    {
        public override Vector2 GetUV(Vector3 position)
        {
            float u = (position.X + 75) / 150;
            float v = (45 - position.Y) / 100;
            return new Vector2(u - MathF.Floor(u), v - MathF.Floor(v));
        }
    }
}
