using System;
using System.Collections.Generic;
using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

/// <summary>A university-style sphere lattice unfurls into a synthwave spiral across two prints.</summary>
public sealed class OrderEscapeScene : Scene
{
    public IReadOnlyList<Sphere> OrderedSpheres => ordered;
    public IReadOnlyList<Sphere> EscapingSpheres => escaping;
    private readonly List<Sphere> ordered = new();
    private readonly List<Sphere> escaping = new();

    public OrderEscapeScene()
    {
        Vector3[] palette = { new(.02f, .85f, 1f), new(.98f, .04f, .48f),
            new(.48f, .12f, 1f), new(1f, .38f, .2f), new(.1f, .55f, 1f) };
        Material Finish(int color) => new(palette[color % palette.Length], .76f, .18f, .2f)
            { Shininess = 100, Emission = .18f };
        void Bead(List<Sphere> group, Vector3 position, float radius, int color)
        {
            var bead = new Sphere(position, radius, Finish(color), null);
            group.Add(bead);
            AddObject(bead);
        }

        AddObject(new Primitives.Plane(Vector3.UnitY, 0,
            new Material(new(.095f, .045f, .17f), .48f, .65f, .08f)
            { Shininess = 110, Emission = .3f }, null));
        AddObject(new Primitives.Plane(-Vector3.UnitZ, 95,
            new Material(new(.055f, .03f, .14f), .7f, 0, 0) { Emission = .7f }, null));

        // Equal radii and spacing; a slight rigid rotation reveals the depth of the lattice.
        Quaternion rotation = Quaternion.CreateFromYawPitchRoll(-.28f, .08f, 0);
        for (int layer = 0; layer < 3; layer++)
            for (int row = 0; row < 5; row++)
                for (int column = 0; column < 4; column++)
                {
                    Vector3 local = new((column - 1.5f) * 2.3f, (row - 2) * 2.3f, (layer - 1) * 2.3f);
                    Bead(ordered, new Vector3(17.5f, 8, 11) + Vector3.Transform(local, rotation), .72f,
                        column + row + layer);
                }

        // A real ribbon of departing beads continues through the unprinted wall interval.
        for (int i = 0; i < 18; i++)
        {
            float t = i / 18f;
            Bead(escaping, new(12 - t * 25, 7.8f - t * 2.3f + MathF.Sin(t * MathF.PI) * 1.5f,
                11 - MathF.Sin(t * MathF.PI) * 2), .52f, i / 3);
        }

        // Expanding helix: ordered rhythm remains visible as the spheres gain space.
        for (int i = 1; i <= 48; i++)
        {
            float t = i / 48f;
            float angle = t * MathF.Tau * 2.05f;
            float radius = 4.5f + t * 1.7f;
            Bead(escaping, new(-17.5f + MathF.Cos(angle) * radius, 5.5f + t * 10,
                11 + MathF.Sin(angle) * radius * .8f), .46f - t * .08f, i / 3);
        }
        Bead(escaping, new(-23.5f, 17.2f, 10), .36f, 1);
        Bead(escaping, new(-19.5f, 18.4f, 13), .31f, 0);
        Bead(escaping, new(-14, 17.3f, 8), .38f, 3);

        Material chrome = new(new(.7f, .75f, .9f), .06f, .94f, .18f) { Shininess = 150 };
        AddObject(new Sphere(new(20.5f, 2.8f, 1), 2.8f, chrome, null));
        AddObject(new Sphere(new(-19.5f, 3, 3), 3, chrome, null));

        // Off-camera luminous ribbons reflect in the metal; point lights illuminate the forms.
        void Panel(Vector3 min, Vector3 max, Vector3 color)
            => AddObject(new Box(min, max, new Material(color) { Emission = 1 }, null));
        Panel(new(60, 8, -32), new(60.15f, 34, 10), new(.03f, .85f, 1f));
        Panel(new(-60.15f, 8, -32), new(-60, 34, 10), new(1f, .05f, .5f));
        Panel(new(-35, 40, -15), new(35, 40.1f, -8), new(.6f, .32f, 1f));
        Panel(new(-32, 25, -35), new(32, 27, -34.9f), new(1f, .48f, .3f));
        AddLight(new Light(new(25, 24, -16), float.MinValue, new Material(new(.22f, .85f, 1.15f))));
        AddLight(new Light(new(-25, 21, -10), float.MinValue, new Material(new(1.15f, .3f, .65f))));
        AddLight(new Light(new(0, 26, 25), float.MinValue, new Material(new(.58f, .38f, .95f))));
        Camera.Target = new(0, 6, 9);
    }
}
