using System;
using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

/// <summary>Two sculptures exchanging real reflections across a hidden strip of one camera view.</summary>
public sealed class ConversationScene : Scene
{
    public ConversationScene()
    {
        Material copper = new(new(.88f, .42f, .19f), .58f, .42f, .22f) { Shininess = 80, Emission = .06f };
        Material blue = new(new(.08f, .38f, .56f), .65f, .32f, .2f) { Shininess = 90, Emission = .1f };
        Material chrome = new(new(.6f, .68f, .74f), .08f, .92f, .18f) { Shininess = 140 };
        Material amber = new(new(1f, .62f, .22f), .7f, .25f, .2f) { Shininess = 70, Emission = .12f };
        Material stone = new(new(.18f, .23f, .27f), .7f, .15f, .08f) { Shininess = 70, Emission = .08f };

        AddObject(new Primitives.Plane(Vector3.UnitY, 0,
            new Material(new(.11f, .15f, .19f), .5f, .6f, .12f) { Shininess = 100, Emission = .25f }, null));
        AddObject(new Primitives.Plane(-Vector3.UnitZ, 85,
            new Material(new(.11f, .16f, .2f), .65f, 0, 0) { Emission = .45f }, null));

        // Screen-left corresponds to positive world X with this camera basis.
        Vector3 ringCenter = new(16, 8.3f, 8);
        AddObject(new RotatedPrimitive(new Torus(copper, null, 4.7f, .42f),
            Quaternion.CreateFromYawPitchRoll(.25f, 1.35f, -.18f), ringCenter));
        AddObject(new Cylinder(new(16, .4f, 8), 3.4f, .8f, stone, null));
        AddObject(new Cylinder(new(16, .83f, 8), 3.1f, .06f, copper, null));
        for (int i = 0; i < 5; i++)
        {
            float t = i / 4f;
            AddObject(new Sphere(new(17.5f - t * 3, 5 + t * 6, 7.7f + MathF.Sin(t * MathF.PI) * .8f),
                .32f + t * .16f, amber, null));
        }

        Vector3 sculptureCenter = new(-13, 8, 12);
        AddObject(new Cylinder(new(-13, .5f, 12), 3.3f, 1, stone, null));
        AddObject(new Cylinder(new(-13, 1.03f, 12), 3.1f, .06f, copper, null));
        for (int i = 0; i < 3; i++)
        {
            float y = 3.4f + i * 3.2f;
            float size = 1.8f - i * .18f;
            AddObject(new RotatedPrimitive(new Box(new(-size, -size, -size), new(size, size, size), blue, null),
                Quaternion.CreateFromYawPitchRoll(.55f + i * .35f, .15f, .65f), new(-13, y, 12)));
        }
        AddObject(new Sphere(new(-22, 4.3f, 9), 4.3f, chrome, null));
        AddObject(new Sphere(new(-15, .65f, 1), .65f, copper, null));

        // Aim the mirror normal halfway between the viewer and the opposite sculpture.
        Vector3 mirrorCenter = new(23, 6, 5);
        Vector3 viewer = new(0, 7, -28);
        Vector3 normal = Vector3.Normalize(Vector3.Normalize(viewer - mirrorCenter) +
            Vector3.Normalize(sculptureCenter - mirrorCenter));
        Vector3 axis = Vector3.Normalize(Vector3.Cross(Vector3.UnitZ, normal));
        Quaternion orientation = Quaternion.CreateFromAxisAngle(axis, MathF.Acos(normal.Z));
        AddObject(new RotatedPrimitive(new Box(new(-3.1f, -5.5f, -.15f), new(3.1f, 5.5f, .15f), copper, null),
            orientation, mirrorCenter));
        AddObject(new RotatedPrimitive(new Box(new(-2.85f, -5.25f, .15f), new(2.85f, 5.25f, .18f),
            new Material(new(.3f, .35f, .4f), .02f, .98f, .03f) { Shininess = 120 }, null), orientation, mirrorCenter));

        // Long softbox shapes supply crisp, broad highlights in the mirrors and metal.
        void Panel(Vector3 min, Vector3 max, Vector3 color)
            => AddObject(new Box(min, max, new Material(color) { Emission = 1 }, null));
        Panel(new(-60, 12, -35), new(-59.8f, 34, 8), new(.5f, .8f, 1f));
        Panel(new(59.8f, 12, -35), new(60, 34, 8), new(1f, .78f, .52f));
        Panel(new(-40, 40, -18), new(40, 40.1f, -5), new(.8f, .9f, 1f));
        AddLight(new Light(new(12, 22, -12), float.MinValue, new Material(new(1f, .86f, .7f))));
        AddLight(new Light(new(-24, 16, -8), float.MinValue, new Material(new(.6f, .83f, 1f))));
        AddLight(new Light(new(0, 19, 25), float.MinValue, new Material(new(.7f, .82f, 1f))));
        Camera.Target = new(0, 5, 7);
    }
}
