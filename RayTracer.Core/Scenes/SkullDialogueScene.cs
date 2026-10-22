using System;
using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

/// <summary>Two monumental skulls in a photographic studio, inspired by Irving Penn's sculptural still lifes.</summary>
public sealed class SkullDialogueScene : Scene
{
    public Mesh PearlSkull { get; }
    public Mesh SilverSkull { get; }
    public const float InwardYawDegrees = 65;

    public SkullDialogueScene()
    {
        Material pearl = new(new(.75f, .73f, .69f), .48f, .55f, .09f) { Shininess = 110, Emission = .04f };
        Material silver = new(new(.32f, .33f, .34f), .16f, .88f, .08f) { Shininess = 140, Emission = .025f };
        float yaw = InwardYawDegrees * MathF.PI / 180;
        PearlSkull = SkullEchoScene.Skull(pearl, 20.5f, 18.7f, 6, yaw, centerY: 0.4f);
        SilverSkull = SkullEchoScene.Skull(silver, 20.5f, -18.7f, 6, -yaw, centerY: 0.4f);
        AddObject(PearlSkull); AddObject(SilverSkull);
        Environment = StudioEnvironment;
        AddLighting(new Light(new(-28, 32, -25), float.MinValue, new Material(new(.88f, .86f, .82f))));
        AddLighting(new Light(new(28, 15, -25), float.MinValue, new Material(new(.4f, .41f, .42f))));
        Camera.Target = new(0, 0, 6);
    }

    private static Vector3 StudioEnvironment(Vector3 direction)
    {
        Vector3 d = Vector3.Normalize(direction);
        float paper = System.Math.Clamp((d.Z + .35f) / 1.2f, 0, 1);
        Vector3 tone = Vector3.Lerp(new(.105f, .11f, .115f), new(.71f, .695f, .665f), paper);
        tone *= 1 + .055f * d.Y;
        if (d.Z < -.08f)
        {
            float u = d.X / -d.Z, v = d.Y / -d.Z;
            // Reflected studio apertures, represented as illumination rather than objects.
            float key = MathF.Exp(-MathF.Pow(MathF.Abs((u - .8f) / .65f), 8)
                - MathF.Pow(MathF.Abs((v - .65f) / 1.2f), 8));
            float fill = MathF.Exp(-MathF.Pow(MathF.Abs((u + 1.25f) / .38f), 8)
                - MathF.Pow(MathF.Abs((v - .35f) / 1.55f), 8));
            tone = Vector3.Lerp(tone, new(1.02f, 1f, .96f), key);
            tone = Vector3.Lerp(tone, new(.62f, .64f, .66f), fill);
        }
        return tone;
    }
}
