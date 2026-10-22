using System.Linq;
using System.Numerics;
using RayTracer.Core.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;
using RayTracer.Core.Scenes;

namespace RayTracer.Core.Tests;

[TestClass]
public class SkullChromaticSceneTests
{
    [TestMethod]
    public void Composition_ContainsOnlyTwoSkullsWithPolishedBrowSurfaces()
    {
        var scene = new SkullChromaticScene();
        var objects = scene.Cast<Primitive>().ToArray();
        Assert.HasCount(4, objects);
        Assert.IsTrue(scene.CopperBrow.Triangles.Count > 0);
        Assert.IsTrue(scene.BlueBrow.Triangles.Count > 0);
        Assert.IsFalse(scene.CopperSkull.Triangles.Intersect(scene.CopperBrow.Triangles).Any());
        Assert.IsFalse(scene.BlueSkull.Triangles.Intersect(scene.BlueBrow.Triangles).Any());
        Assert.IsTrue(objects.All(p => p is Mesh));
        Assert.AreEqual(3, scene.Lighting.Count());
        Assert.IsNotNull(scene.Environment);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void OppositeSkull_IsVisibleThroughReflectionOnly(bool rightPanel)
    {
        var scene = new SkullChromaticScene();
        var opposite = rightPanel ? scene.CopperSkull : scene.BlueSkull;
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
            opposite.Material = new Material(Vector3.Zero);
            using var after = new Engine(scene, options).Render();
            int changed = 0;
            int strong = 0;
            for (int y = 0; y < options.Height; y++)
                for (int x = 0; x < options.Width; x++)
                {
                    var a = before[x,y]; var b = after[x,y];
                    if (a != b) changed++;
                    if (System.Math.Abs(a.R - b.R) > 40 || System.Math.Abs(a.G - b.G) > 40 ||
                        System.Math.Abs(a.B - b.B) > 40) strong++;
                }
            if (depth == 1) Assert.AreEqual(0, changed);
            else Assert.IsGreaterThan(150, strong, "The neighbouring skull must make a substantial, high-contrast reflection.");
        }
    }
}
