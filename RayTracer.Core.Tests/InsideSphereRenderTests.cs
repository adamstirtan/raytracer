using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;
using RayTracer.Core.Scenes;
using SixLabors.ImageSharp.PixelFormats;

namespace RayTracer.Core.Tests;

[TestClass]
public class InsideSphereRenderTests
{
    private static Scene CreateScene()
    {
        var scene = new Scene();
        scene.AddObject(new Sphere(Vector3.Zero, 2f,
            new Material(new Vector3(0.8f, 0.2f, 0.1f), 1f, 0f, 0f), null));
        scene.AddLight(new Light(Vector3.Zero, float.MinValue, new Material(new Vector3(0.5f))));
        return scene;
    }

    private static Rgba32 RenderCenter(Scene scene)
    {
        var options = new RenderOptions
        {
            Width = 1, Height = 1, TraceDepth = 1, DisableReflections = true,
            CameraPosition = Vector3.Zero, CameraTarget = Vector3.UnitZ
        };
        using var image = new Engine(scene, options).Render();
        return image[0, 0];
    }

    [TestMethod]
    public void CameraInsideSphere_SeesExitLitByInteriorLight()
    {
        // An inward-facing normal receives light from the center; the outward normal does not.
        Assert.AreEqual(new Rgba32(0.4f, 0.1f, 0.05f), RenderCenter(CreateScene()));
    }

    [TestMethod]
    public void CloserObject_WinsOverContainingSphereExit()
    {
        Scene scene = CreateScene();
        scene.AddObject(new Sphere(Vector3.UnitZ, 0.25f,
            new Material(Vector3.UnitY, 1f, 0f, 0f), null));

        Assert.AreEqual(new Rgba32(0f, 0.5f, 0f), RenderCenter(scene));
    }
}
