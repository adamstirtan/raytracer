using System;
using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Physics;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

/// <summary>A frozen instant of the existing falling-sphere simulation, lit for a synthwave diptych.</summary>
public sealed class ImpactScene : Scene
{
    public double SimulationTimeSeconds { get; }
    public SphereDynamics Dynamics { get; }

    public ImpactScene(double seconds = 3.15)
    {
        if (!double.IsFinite(seconds) || seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));
        SimulationTimeSeconds = seconds;
        var falling = new FallingSpheresScene();
        Dynamics = falling.Dynamics;
        Dynamics.Advance(seconds);
        Vector3[] palette = { new(.02f, .85f, 1f), new(1f, .05f, .5f), new(.5f, .12f, 1f),
            new(1f, .42f, .22f), new(.12f, .55f, 1f), new(.75f, .8f, .95f) };
        int index = 0;
        foreach (var body in Dynamics.Bodies)
        {
            bool chrome = index % palette.Length == 5;
            body.Sphere.Material = new Material(palette[index % palette.Length], chrome ? .08f : .72f,
                chrome ? .9f : .28f, .2f) { Shininess = chrome ? 140 : 100, Emission = chrome ? 0 : .18f };
            AddObject(body.Sphere);
            index++;
        }
        AddObject(new Primitives.Plane(Vector3.UnitY, 0,
            new Material(new(.09f, .045f, .17f), .5f, .65f, .08f) { Shininess = 110, Emission = .3f }, null));
        AddObject(new Primitives.Plane(-Vector3.UnitZ, 95,
            new Material(new(.055f, .03f, .14f), .7f, 0, 0) { Emission = .7f }, null));
        void Panel(Vector3 min, Vector3 max, Vector3 color)
            => AddObject(new Box(min, max, new Material(color) { Emission = 1 }, null));
        Panel(new(50, 8, -32), new(50.15f, 34, 10), new(.03f, .85f, 1f));
        Panel(new(-50.15f, 8, -32), new(-50, 34, 10), new(1f, .05f, .5f));
        Panel(new(-30, 36, -18), new(30, 36.1f, -8), new(.65f, .4f, 1f));
        AddLight(new Light(new(18, 24, -16), float.MinValue, new Material(new(.35f, .9f, 1.1f))));
        AddLight(new Light(new(-18, 21, -10), float.MinValue, new Material(new(1.1f, .35f, .7f))));
        AddLight(new Light(new(0, 26, 25), float.MinValue, new Material(new(.65f, .45f, 1f))));
        Camera.Target = new(0, 4.2f, 11);
    }
}
