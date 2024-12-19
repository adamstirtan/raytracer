using Microsoft.VisualStudio.TestTools.UnitTesting;

using RayTracer.Core.Scenes;

namespace RayTracer.Core.Tests;

[TestClass]
public class SphereSceneTest
{
    [TestMethod]
    public void RenderTest()
    {
        Engine engine = new(new SphereScene());

        var render = engine.Render(new RenderOptions
        {
            DisableReflections = true,
            TraceDepth = 1,
            Width = 800,
            Height = 800
        });

        Assert.IsNotNull(render);
    }
}