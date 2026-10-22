using System.Linq;
using System.Numerics;
using RayTracer.Core.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;
using RayTracer.Core.Scenes;

namespace RayTracer.Core.Tests;

[TestClass]
public class SkullDialogueSceneTests
{
    [TestMethod]
    public void Composition_ContainsOnlyTwoSkullMeshes()
    {
        var scene = new SkullDialogueScene();
        var objects = scene.Cast<Primitive>().ToArray();
        Assert.AreEqual(2, objects.Length);
        Assert.IsTrue(objects.All(p => p is Mesh));
        Assert.AreEqual(2, scene.Lighting.Count());
        Assert.IsNotNull(scene.Environment);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void OppositeSkull_IsVisibleThroughReflectionOnly(bool rightPanel)
    {
        var scene = new SkullDialogueScene();
        var opposite = rightPanel ? scene.PearlSkull : scene.SilverSkull;
        Material original = opposite.Material;
        foreach (int depth in new[] { 1, 6 })
        {
            var options = new RenderOptions
            {
                Width = 124, Height = 164, FullWidth = 334, HorizontalOffset = rightPanel ? 210 : 0,
                CameraPosition = new(0,0,-30), CameraTarget = new(0,0,6),
                TraceDepth = depth, SamplesPerPixel = 4, DisableReflections = false
            };
            opposite.Material = original;
            using var before = new Engine(scene, options).Render();
            opposite.Material = new Material(Vector3.UnitY) { Emission = 1 };
            using var after = new Engine(scene, options).Render();
            int changed = 0;
            for (int y = 0; y < options.Height; y++)
                for (int x = 0; x < options.Width; x++)
                    if (before[x,y] != after[x,y]) changed++;
            if (depth == 1) Assert.AreEqual(0, changed);
            else Assert.IsGreaterThan(0, changed, "The neighbouring skull must contribute to the reflections.");
        }
    }
}
