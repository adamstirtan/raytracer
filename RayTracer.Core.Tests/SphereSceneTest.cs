using Microsoft.VisualStudio.TestTools.UnitTesting;

using RayTracer.Core.Scenes;

namespace RayTracer.Core.Tests
{
    [TestClass]
    public class SphereSceneTest
    {
        [TestMethod]
        public void RenderTest()
        {
            var engine = new Engine();

            engine.LoadScene(new SphereScene());

            var render = engine.Render(800, 600);

            Assert.IsNotNull(render);
        }
    }
}