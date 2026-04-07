using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;

namespace RayTracer.Core.Tests
{
    [TestClass]
    public class TextureTests
    {
        [TestMethod]
        public void Sample_ReturnsValidColor_ForWrappedCoords()
        {
            // Ensure texture is available in the test working directory
            EnsureTextureAvailable("green-felt.jpg");

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

        private void EnsureTextureAvailable(string fileName)
        {
            string destDir = System.IO.Path.Combine(System.AppContext.BaseDirectory, "Textures");
            System.IO.Directory.CreateDirectory(destDir);
            string dest = System.IO.Path.Combine(destDir, fileName);
            if (!System.IO.File.Exists(dest))
            {
                // locate source in repo
                string repoRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.AppContext.BaseDirectory, "..", "..", "..", ".."));
                string source = System.IO.Path.Combine(repoRoot, "RayTracer.Core", "Textures", fileName);
                System.IO.File.Copy(source, dest, overwrite: true);
            }
        }
    }
}
