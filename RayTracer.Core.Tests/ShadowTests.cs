using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;
using RayTracer.Core.Scenes;
using SixLabors.ImageSharp.PixelFormats;

namespace RayTracer.Core.Tests;

[TestClass]
public class ShadowTests
{
    private static Rgba32 RenderGround(float? blockerHeight, bool specularOnly = false, bool secondLight = false)
    {
        var scene = new Scene();
        scene.AddObject(new RayTracer.Core.Primitives.Plane(Vector3.UnitY, 0f,
            new Material(Vector3.One, specularOnly ? 0f : 1f, 0f, specularOnly ? 1f : 0f), null));
        scene.AddLight(new Light(new Vector3(0, 3, 0), float.MinValue, new Material(new Vector3(0.5f))));
        if (blockerHeight.HasValue)
            scene.AddObject(new Sphere(new Vector3(0, blockerHeight.Value, 0), 0.4f,
                new Material(Vector3.One), null));
        if (secondLight)
            scene.AddLight(new Light(new Vector3(3, 3, 0), float.MinValue, new Material(new Vector3(0.25f))));

        var options = new RenderOptions
        {
            Width = 1, Height = 1, TraceDepth = 1, DisableReflections = true,
            CameraPosition = new Vector3(0, 1, -3), CameraTarget = Vector3.Zero
        };
        using var image = new Engine(scene, options).Render();
        return image[0, 0];
    }

    [TestMethod]
    public void BlockerBetweenGroundAndLight_RemovesDiffuseLighting()
    {
        Assert.IsGreaterThan((byte)0, RenderGround(null).R);
        Assert.AreEqual((byte)0, RenderGround(1.5f).R);
    }

    [TestMethod]
    public void BlockerBetweenGroundAndLight_RemovesSpecularLighting()
    {
        Assert.IsGreaterThan((byte)0, RenderGround(null, specularOnly: true).R);
        Assert.AreEqual((byte)0, RenderGround(1.5f, specularOnly: true).R);
    }

    [TestMethod]
    public void BlockerBeyondLight_DoesNotCastShadow()
    {
        Assert.AreEqual(RenderGround(null), RenderGround(4.5f));
    }

    [TestMethod]
    public void OccludedLight_DoesNotSuppressOtherLights()
    {
        Assert.IsGreaterThan((byte)0, RenderGround(1.5f, secondLight: true).R);
    }

}
