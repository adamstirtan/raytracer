using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;
using RayTracer.Core.Scenes;

namespace RayTracer.Core.Tests;

[TestClass]
public class OrderEscapeSceneTests
{
    [TestMethod]
    public void FloatingBeads_AreSeparatedAndLatticeRadiiAreEqual()
    {
        var scene = new OrderEscapeScene();
        Assert.HasCount(60, scene.OrderedSpheres);
        var beads = scene.OrderedSpheres.Concat(scene.EscapingSpheres).ToArray();
        foreach (var sphere in scene.OrderedSpheres) Assert.AreEqual(.72f, sphere.Radius);
        for (int i = 0; i < beads.Length; i++)
            for (int j = i + 1; j < beads.Length; j++)
                Assert.IsGreaterThan(beads[i].Radius + beads[j].Radius,
                    Vector3.Distance(beads[i].Center, beads[j].Center), $"Beads {i} and {j} overlap.");
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void OppositeArrangement_AppearsThroughReflectionsOnly(bool rightPanel)
    {
        var scene = new OrderEscapeScene();
        Sphere[] opposite = rightPanel ? scene.OrderedSpheres.ToArray()
            : scene.EscapingSpheres.Where(sphere => sphere.Center.X < -8).ToArray();
        var original = opposite.Select(sphere => sphere.Material).ToArray();
        foreach (int depth in new[] { 1, 5 })
        {
            var options = new RenderOptions
            {
                Width = 124, Height = 164, FullWidth = 334, HorizontalOffset = rightPanel ? 210 : 0,
                CameraPosition = new(0, 8, -28), CameraTarget = new(0, 6, 9),
                DisableReflections = false, TraceDepth = depth, SamplesPerPixel = 4
            };
            for (int i = 0; i < opposite.Length; i++) opposite[i].Material = original[i];
            using var before = new Engine(scene, options).Render();
            foreach (var sphere in opposite) sphere.Material = new Material(Vector3.UnitY) { Emission = 1 };
            using var after = new Engine(scene, options).Render();
            int changed = 0;
            for (int y = 0; y < options.Height; y++)
                for (int x = 0; x < options.Width; x++)
                    if (before[x, y] != after[x, y]) changed++;
            if (depth == 1) Assert.AreEqual(0, changed, "The opposite arrangement is outside this panel's camera crop.");
            else Assert.IsGreaterThan(0, changed, "The opposite arrangement must appear in reflected rays.");
        }
    }
}
