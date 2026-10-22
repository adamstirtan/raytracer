using System.Linq;
using System;
using System.IO;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;
using RayTracer.Core.Scenes;

namespace RayTracer.Core.Tests;

[TestClass]
public class SkullBoneSceneTests
{
    [TestMethod]
    public void Composition_HasTexturedBoneAndDarkReflectiveMetalOnly()
    {
        var scene = new SkullBoneScene();
        Assert.HasCount(2, scene.Cast<Primitive>().ToArray());
        Assert.IsNotNull(scene.BoneSkull.Texture);
        Assert.IsNotNull(scene.BoneSkull.TriplanarTextureScale);
        Assert.IsTrue(scene.BoneSkull.Material.Reflection < .05f);
        Assert.IsTrue(scene.GreySkull.Material.Reflection > .8f);
        Assert.IsTrue(scene.GreySkull.Material.Color.Length() < .3f);
        var original = Mesh.FromObj(Path.Combine(AppContext.BaseDirectory, "assets", "skull.obj"),
            new Material(Vector3.One), normalize: false);
        foreach (var skull in new[] { scene.BoneSkull, scene.GreySkull })
        {
            Assert.AreEqual(original.Vertices.Count, skull.Vertices.Count);
            CollectionAssert.AreEqual(original.Triangles, skull.Triangles,
                "Both sculptures must retain the original full skull topology.");
        }
    }

    [TestMethod]
    public void BoneFace_ContributesToNaturalMetalReflections()
    {
        var scene = new SkullBoneScene();
        var texture = scene.BoneSkull.Texture;
        Material material = scene.BoneSkull.Material;
        foreach (int depth in new[] { 1, 6 })
        {
            var options = new RenderOptions
            {
                Width = 124, Height = 164, FullWidth = 334, HorizontalOffset = 210,
                CameraPosition = new(0,0,-30), CameraTarget = new(0,0,6),
                TraceDepth = depth, SamplesPerPixel = 4, DisableReflections = false
            };
            scene.BoneSkull.Texture = texture;
            scene.BoneSkull.Material = material;
            using var before = new Engine(scene, options).Render();
            scene.BoneSkull.Texture = null;
            scene.BoneSkull.Material = new Material(Vector3.Zero);
            using var after = new Engine(scene, options).Render();
            int changed = 0, strong = 0;
            for (int y = 0; y < options.Height; y++)
                for (int x = 0; x < options.Width; x++)
                {
                    var a = before[x,y]; var b = after[x,y];
                    if (a != b) changed++;
                    if (System.Math.Abs(a.R - b.R) > 40) strong++;
                }
            if (depth == 1) Assert.AreEqual(0, changed);
            else Assert.IsGreaterThan(0, strong, "The curved metal surface must reflect the bone skull.");
        }
    }
}
