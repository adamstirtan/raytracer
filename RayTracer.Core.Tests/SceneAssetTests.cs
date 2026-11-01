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
            Assert.IsNotNull(JsonSceneLoader.Load(JsonSceneLoader.FindBundled("billiards")!));
            Assert.IsNotNull(JsonSceneLoader.Load(JsonSceneLoader.FindBundled("textured-sphere")!));
            Assert.IsNotNull(JsonSceneLoader.Load(JsonSceneLoader.FindBundled("mesh")!));
            Assert.IsNotNull(JsonSceneLoader.Load(JsonSceneLoader.FindBundled("hand")!));
            Assert.IsNotNull(JsonSceneLoader.Load(JsonSceneLoader.FindBundled("orbital")!));
            Assert.IsNotNull(JsonSceneLoader.Load(JsonSceneLoader.FindBundled("future-city")!));
            Assert.IsNotNull(new SynthwaveScene());
        }
        finally
        {
            Environment.CurrentDirectory = previous;
            Directory.Delete(directory);
        }
    }
}
