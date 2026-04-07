using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Scenes;
using RayTracer.Core;
using System.Numerics;

namespace RayTracer.Core.Tests
{
    [TestClass]
    public class BilliardsSceneTests
    {
        [TestMethod]
        public void BilliardsScene_CanConstruct_AndEngineRendersSmallImage()
        {
            var scene = new BilliardsScene();
            var options = new RenderOptions
            {
                Width = 40,
                Height = 30,
                TraceDepth = 1,
                CameraPosition = new Vector3(0, 1, -4),
                CameraTarget = new Vector3(0, 0.2f, 1.8f),
                DisableReflections = true
            };

            var engine = new Engine(scene, options);
            var img = engine.Render(); // should not throw
            Assert.IsNotNull(img);
            Assert.AreEqual(40, img.Width);
            Assert.AreEqual(30, img.Height);
        }
    }
}
