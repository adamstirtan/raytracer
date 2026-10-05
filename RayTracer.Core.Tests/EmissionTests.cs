using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;
using RayTracer.Core.Scenes;

namespace RayTracer.Core.Tests;

[TestClass]
public class EmissionTests
{
    [TestMethod]
    public void EmissiveSurface_RemainsVisibleWithoutLights()
    {
        var material = new Material(new Vector3(.5f, .25f, .125f)) { Emission = 1 };
        var scene = new Scene();
        scene.AddObject(new RayTracer.Core.Primitives.Plane(-Vector3.UnitZ, 5, material, null));
        var options = new RenderOptions
        {
            Width = 1, Height = 1, TraceDepth = 1, DisableReflections = true,
            CameraPosition = Vector3.Zero, CameraTarget = Vector3.UnitZ
        };
        using var lit = new Engine(scene, options).Render();
        Assert.IsTrue(lit[0, 0].R > lit[0, 0].G && lit[0, 0].G > lit[0, 0].B);
        material.Emission = 0;
        using var dark = new Engine(scene, options).Render();
        Assert.AreEqual((byte)0, dark[0, 0].R);
    }
}
