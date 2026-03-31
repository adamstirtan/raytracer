using System.Numerics;
using RayTracer.Core;
using RayTracer.Core.Scenes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RayTracer.Core.Tests
{
    [TestClass]
    public class CameraOptionsTests
    {
        [TestMethod]
        public void Engine_Uses_RenderOptions_CameraTarget_When_Set()
        {
            var scene = new SphereScene();
            var options = new RenderOptions
            {
                Width = 100,
                Height = 80,
                CameraPosition = new Vector3(0,0,-5),
                CameraTarget = new Vector3(10,10,10),
                TraceDepth = 1,
                DisableReflections = true
            };

            var engine = new Engine(scene, options);
            var img = engine.Render();
            Assert.AreEqual(options.Width, img.Width);
            Assert.AreEqual(options.Height, img.Height);
        }
    }
}
