using System;
using System.IO;
using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

/// <summary>Natural ivory bone beside smoked grey metal reflecting the bone face.</summary>
public sealed class SkullBoneScene : Scene
{
    public Mesh BoneSkull { get; }
    public Mesh GreySkull { get; }

    public SkullBoneScene()
    {
        Material bone = new(Vector3.One, .7f, .015f, .025f) { Shininess = 35, Emission = .085f };
        Material grey = new(new(.15f, .155f, .16f), .16f, .82f, .075f) { Shininess = 140, Emission = .015f };
        float yaw = 60 * MathF.PI / 180;
        BoneSkull = SkullEchoScene.Skull(bone, 20.5f, 18.7f, 6, yaw, centerY: .4f);
        BoneSkull.Texture = new Texture(Path.Combine(AppContext.BaseDirectory, "Textures", "Skull", "bone-albedo.png"));
        BoneSkull.TriplanarTextureScale = 1f / 8;
        BoneSkull.TextureOrigin = new(18.7f, .4f, 6);
        GreySkull = SkullEchoScene.Skull(grey, 20.5f, -18.7f, 6, -yaw, centerY: .4f);
        AddObject(BoneSkull);
        AddObject(GreySkull);
        Environment = StudioEnvironment;
        AddLighting(new Light(new(-25, 26, -24), float.MinValue, new Material(new(.85f, .83f, .79f))));
        AddLighting(new Light(new(25, 20, -24), float.MinValue, new Material(new(.53f, .54f, .55f))));
        AddLighting(new Light(new(0, 5, -24), float.MinValue, new Material(new(.13f, .13f, .13f))));
        Camera.Target = new(0, 0, 6);
    }

    private static Vector3 StudioEnvironment(Vector3 direction)
    {
        Vector3 d = Vector3.Normalize(direction);
        float paper = System.Math.Clamp((d.Z + .35f) / 1.2f, 0, 1);
        Vector3 tone = Vector3.Lerp(new(.105f, .11f, .115f), new(.47f, .455f, .425f), paper);
        tone *= 1 + .045f * d.Y;
        if (d.Z < -.08f)
        {
            float u = d.X / -d.Z, v = d.Y / -d.Z;
            float aperture = MathF.Exp(-MathF.Pow(MathF.Abs(u / 1.35f), 8)
                - MathF.Pow(MathF.Abs((v - 1.1f) / .5f), 8));
            tone = Vector3.Lerp(tone, new(.8f, .79f, .76f), aperture);
        }
        return tone;
    }
}
