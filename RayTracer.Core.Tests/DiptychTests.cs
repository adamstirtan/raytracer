using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;
using RayTracer.Core.Scenes;

namespace RayTracer.Core.Tests;

[TestClass]
public class DiptychTests
{
    [TestMethod]
    public void PanelPixels_MatchTheSameRegionOfOneWideCamera()
    {
        Scene scene = new ConversationScene();
        RenderOptions Options(int width, int offset) => new()
        {
            Width = width, Height = 82, FullWidth = 167, HorizontalOffset = offset,
            CameraPosition = new(0, 7, -28), CameraTarget = new(0, 5, 7),
            DisableReflections = false, TraceDepth = 4, SamplesPerPixel = 4
        };
        using var wide = new Engine(scene, Options(167, 0)).Render();
        foreach (int offset in new[] { 0, 105 })
        {
            using var panel = new Engine(scene, Options(62, offset)).Render();
            for (int y = 0; y < 82; y++)
                for (int x = 0; x < 62; x++)
                    Assert.AreEqual(wide[x + offset, y], panel[x, y]);
        }
    }

    [TestMethod]
    public void LeftRing_ChangesTheRightPanelOnlyThroughReflections()
    {
        var scene = new ConversationScene();
        var ring = scene.First<Primitive>(item => item.GetPrimitiveType() == PrimitiveType.Torus);
        var original = ring.Material;
        var diagnostic = new Material(new(1, 0, 1)) { Emission = 1 };
        foreach (int depth in new[] { 1, 5 })
        {
            var options = new RenderOptions
            {
                Width = 124, Height = 164, FullWidth = 334, HorizontalOffset = 210,
                CameraPosition = new(0, 7, -28), CameraTarget = new(0, 5, 7),
                DisableReflections = false, TraceDepth = depth, SamplesPerPixel = 4
            };
            ring.Material = original;
            using var before = new Engine(scene, options).Render();
            ring.Material = diagnostic;
            using var after = new Engine(scene, options).Render();
            int changed = 0;
            for (int y = 0; y < options.Height; y++)
                for (int x = 0; x < options.Width; x++)
                    if (before[x, y] != after[x, y]) changed++;
            if (depth == 1) Assert.AreEqual(0, changed, "The left ring must be outside the right panel's direct view.");
            else Assert.IsGreaterThan(0, changed, "The opposite ring must appear in the right panel's reflections.");
        }
    }
}
