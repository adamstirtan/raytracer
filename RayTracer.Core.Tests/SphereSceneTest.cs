using System.Numerics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RayTracer.Core.Scenes;

namespace RayTracer.Core.Tests;

[TestClass]
public class SphereSceneTest
{
    [TestMethod]
    public void RenderTest()
    {
        RenderOptions options = new()
        {
            CameraPosition = new Vector3(0, 0, -5),
            DisableReflections = true,
            TraceDepth = 1,
            Width = 800,
            Height = 800
        };

        Engine engine = new(new SphereScene(), options);

        var render = engine.Render();

        Assert.IsNotNull(render);
    }
}