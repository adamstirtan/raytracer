using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;

namespace RayTracer.Core.Tests
{
    [TestClass]
    public class TextureTests
    {
        [TestMethod]
        public void NegativeCoordinates_WrapToSameTexel()
        {
            var tex = new Texture(System.IO.Path.Combine(System.AppContext.BaseDirectory, "Textures", "earth.jpg"));
            Assert.AreEqual(tex.Sample(0.75f, 0.5f), tex.Sample(-0.25f, -1.5f));
            Assert.AreEqual(tex.Sample(0f, 0f), tex.Sample(-1f, -2f));
        }

        [TestMethod]
        public void Sample_ReturnsValidColor_ForWrappedCoords()
        {
            var tex = new Texture("Textures/green-felt.jpg");
            var uvs = new[] { (0.1f, 0.1f), (1.2f, 0.5f), (2.7f, 3.3f) };

            foreach (var uv in uvs)
            {
                var c = tex.Sample(uv.Item1, uv.Item2);
                Assert.IsTrue(c.X >= 0f && c.X <= 1f);
                Assert.IsTrue(c.Y >= 0f && c.Y <= 1f);
                Assert.IsTrue(c.Z >= 0f && c.Z <= 1f);
            }
        }

    }
}
