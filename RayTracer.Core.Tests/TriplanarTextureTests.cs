using System;
using System.IO;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RayTracer.Core.Tests;

[TestClass]
public class TriplanarTextureTests
{
    [TestMethod]
    public void MeshProjection_SelectsAxesAndBlendsAcrossSurfaceNormals()
    {
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");
        try
        {
            using (var image = new Image<Rgba32>(2, 2))
            {
                image[0,0] = new Rgba32(255, 0, 0);
                image[1,0] = new Rgba32(0, 255, 0);
                image[0,1] = new Rgba32(0, 0, 255);
                image[1,1] = new Rgba32(255, 255, 255);
                image.SaveAsPng(path);
            }
            var mesh = new Mesh(new Material(Vector3.One), new Texture(path))
            {
                TextureOrigin = new(4, 5, 6), TriplanarTextureScale = .5f
            };
            Vector3 position = mesh.TextureOrigin + new Vector3(.5f, .5f, 1.5f);
            Assert.AreEqual(Vector3.UnitY, mesh.SampleSurfaceColor(position, Vector3.UnitX));
            Assert.AreEqual(Vector3.UnitZ, mesh.SampleSurfaceColor(position, Vector3.UnitY));
            Assert.AreEqual(Vector3.UnitX, mesh.SampleSurfaceColor(position, Vector3.UnitZ));
            Assert.AreEqual(new Vector3(0, .5f, .5f), mesh.SampleSurfaceColor(position, new(1, 1, 0)));
            Assert.AreEqual(mesh.SampleSurfaceColor(position, Vector3.UnitX),
                mesh.SampleSurfaceColor(position - new Vector3(2), -Vector3.UnitX));
            Assert.AreEqual(mesh.Texture.SampleTriplanar(Vector3.Zero, Vector3.UnitZ),
                mesh.Texture.SampleTriplanar(Vector3.One, Vector3.UnitZ));
        }
        finally { File.Delete(path); }
    }
}
