using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;
using RayTracer.Core.Scenes;
using SixLabors.ImageSharp.PixelFormats;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RayTracer.Core.Tests;

[TestClass]
public class CameraOptionsTests
{
    private static Rgba32 RenderCenter(Scene scene, Vector3 position, Vector3? target)
    {
        var options = new RenderOptions
        {
            Width = 1, Height = 1, CameraPosition = position, CameraTarget = target,
            TraceDepth = 1, DisableReflections = true
        };
        using var image = new Engine(scene, options).Render();
        return image[0, 0];
    }

    private static Scene CreateTargetScene()
    {
        var scene = new Scene();
        scene.AddLight(new Light(Vector3.Zero, 0.4f, new Material(Vector3.UnitX)));
        scene.AddLight(new Light(new Vector3(5, 0, 0), 0.4f, new Material(Vector3.UnitY)));
        scene.Camera.Target = new Vector3(5, 0, 0);
        return scene;
    }

    [TestMethod]
    public void ExplicitOriginTarget_OverridesSceneTarget()
    {
        Assert.AreEqual(new Rgba32(1f, 0f, 0f),
            RenderCenter(CreateTargetScene(), new Vector3(0, 0, -5), Vector3.Zero));
    }

    [TestMethod]
    public void UnsetTarget_UsesSceneTarget()
    {
        Assert.AreEqual(new Rgba32(0f, 1f, 0f),
            RenderCenter(CreateTargetScene(), new Vector3(0, 0, -5), null));
    }

    [TestMethod]
    public void ExplicitNonzeroTarget_OverridesSceneTarget()
    {
        Scene scene = CreateTargetScene();
        scene.Camera.Target = Vector3.Zero;
        Assert.AreEqual(new Rgba32(0f, 1f, 0f),
            RenderCenter(scene, new Vector3(0, 0, -5), new Vector3(5, 0, 0)));
    }

    [TestMethod]
    public void UnsetOptionsAndSceneTargets_UseOrigin()
    {
        Scene scene = CreateTargetScene();
        scene.Camera.Target = null;
        Assert.AreEqual(new Rgba32(1f, 0f, 0f),
            RenderCenter(scene, new Vector3(0, 0, -5), null));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void CoincidentPositionAndTarget_LookAlongPositiveZ(bool useSceneTarget)
    {
        var position = new Vector3(2, 3, 4);
        var scene = new Scene();
        scene.AddLight(new Light(position + Vector3.UnitZ * 3f, 0.4f, new Material(Vector3.UnitZ)));
        scene.Camera.Target = position;

        Assert.AreEqual(new Rgba32(0f, 0f, 1f),
            RenderCenter(scene, position, useSceneTarget ? null : position));
    }
}
