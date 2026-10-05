using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Scenes;

namespace RayTracer.Core.Tests;

[TestClass]
public class SceneAssetTests
{
    [TestMethod]
    public void ModelAssets_AreIncludedInBuildOutput()
    {
        foreach (string name in new[] { "skull.obj", "teapot.obj", "hand.obj" })
            Assert.IsTrue(File.Exists(Path.Combine(AppContext.BaseDirectory, "assets", name)), name);
    }

    [TestMethod]
    [DoNotParallelize]
    public void Scenes_LoadWhenWorkingDirectoryContainsNoAssets()
    {
        string previous = Environment.CurrentDirectory;
        string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(directory);
        try
        {
            Environment.CurrentDirectory = directory;
            Assert.IsNotNull(new BilliardsScene());
            Assert.IsNotNull(new TexturedSphere());
            Assert.IsNotNull(new MeshScene());
            Assert.IsNotNull(new HandScene());
        }
        finally
        {
            Environment.CurrentDirectory = previous;
            Directory.Delete(directory);
        }
    }
}
