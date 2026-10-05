using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Math;
using RayTracer.Core.Primitives;
using RayTracer.Core.Scenes;

namespace RayTracer.Core.Tests;

[TestClass]
public class SynthwaveSceneTests
{
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void AnimationProgress_KeepsSunStationaryAndRepeatsTerrainExactly(bool distantMountains)
    {
        var scene = new SynthwaveScene(distantMountains);
        var sun = scene.OfType<Disk>().Single();
        Vector3 initial = sun.Center;
        var options = new RenderOptions
        {
            Width = 120, Height = 68, TraceDepth = 1, DisableReflections = true,
            CameraPosition = new Vector3(0,7,-12), CameraTarget = new Vector3(0,9,100)
        };
        using var start = new Engine(scene, options).Render();
        scene.SetAnimationProgress(.37f);
        Assert.AreEqual(initial, sun.Center);
        using var middle = new Engine(scene, options).Render();
        scene.SetAnimationProgress(1);
        using var end = new Engine(scene, options).Render();
        bool changed = false;
        for(int y=0;y<start.Height;y++)
            for(int x=0;x<start.Width;x++)
            {
                Assert.AreEqual(start[x,y],end[x,y]);
                changed |= start[x,y] != middle[x,y];
            }
        Assert.IsTrue(changed);
        Assert.IsTrue(sun.Center.Z > 240 + sun.Radius);
    }

    [TestMethod]
    public void ValleyTraversal_FindsFlatFloorAcrossMultipleCellsAndHonorsDistanceLimit()
    {
        var valley = new SynthwaveScene().OfType<Primitive>().First(p => p.GetPrimitiveType() == PrimitiveType.Mesh);
        Vector3 origin = new(0, 7, -12);
        Vector3 target = new(0, 0, 60);
        float expected = Vector3.Distance(origin, target);
        var ray = new Ray(origin, Vector3.Normalize(target-origin));
        var hit = valley.Intersect(ray, float.MaxValue);
        Assert.AreEqual(RayIntersection.Hit, hit.RayIntersection);
        Assert.AreEqual(expected, hit.Distance, .001f);
        Assert.AreEqual(Vector3.UnitY, hit.Normal);
        Assert.AreEqual(RayIntersection.Miss, valley.Intersect(ray, expected-1).RayIntersection);
    }

    [TestMethod]
    public void ValleyTraversal_AxisParallelRayFindsSlopeWithConsistentNormal()
    {
        var valley = new SynthwaveScene().OfType<Primitive>().First(p => p.GetPrimitiveType() == PrimitiveType.Mesh);
        var ray = new Ray(new Vector3(42, 100, 35), -Vector3.UnitY);
        var hit = valley.Intersect(ray, float.MaxValue);
        Assert.AreEqual(RayIntersection.Hit, hit.RayIntersection);
        Assert.IsTrue(hit.Distance > 0 && hit.Distance < 100);
        Vector3 position = ray.Origin + ray.Direction * hit.Distance;
        Assert.IsTrue(Vector3.Distance(hit.Normal, valley.GetNormal(position)) < .001f);
    }
}
