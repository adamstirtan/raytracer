using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;
using RayTracer.Core.Scenes;
using SixLabors.ImageSharp.PixelFormats;

namespace RayTracer.Core.Tests;

[TestClass]
public class StudioEnvironmentTests
{
    private static RenderOptions Options => new()
    {
        Width = 1, Height = 1, TraceDepth = 2, DisableReflections = false,
        CameraPosition = Vector3.Zero, CameraTarget = Vector3.UnitZ
    };

    [TestMethod]
    public void Environment_IsSampledByCameraAndReflectedRays()
    {
        var scene = new Scene { Environment = direction => direction.Z > 0 ? Vector3.UnitX : Vector3.UnitY };
        using var background = new Engine(scene, Options).Render();
        Assert.AreEqual(new Rgba32(255, 0, 0), background[0, 0]);
        scene.AddObject(new RayTracer.Core.Primitives.Plane(-Vector3.UnitZ, 5,
            new Material(Vector3.One, 0, 1, 0), null));
        using var reflection = new Engine(scene, Options).Render();
        Assert.AreEqual(new Rgba32(0, 255, 0), reflection[0, 0]);
    }

    [TestMethod]
    public void LightingOnly_IlluminatesWithoutAddingGeometry()
    {
        var scene = new Scene();
        scene.AddObject(new RayTracer.Core.Primitives.Plane(-Vector3.UnitZ, 5,
            new Material(Vector3.One, 1, 0, 0), null));
        var light = new Light(Vector3.Zero, float.MinValue, new Material(new Vector3(.5f)));
        scene.AddLighting(light);
        Assert.AreEqual(1, scene.Count<Primitive>());
        using var lit = new Engine(scene, Options).Render();
        Assert.AreEqual(new Rgba32(.5f, .5f, .5f), lit[0, 0]);
    }
}
