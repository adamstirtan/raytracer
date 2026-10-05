using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;
using RayTracer.Core.Primitives;
using RayTracer.Core.Math;
using RayTracer.Core.Materials;

namespace RayTracer.Core.Tests;

[TestClass]
public class SphereIntersectionTests
{
    [TestMethod]
    public void RayStartingInsideSphere_ReturnsPositiveExitDistance()
    {
        var center = new Vector3(2, 3, 4);
        var sphere = new Sphere(center, 2f, new Material(Vector3.One), null);
        var ray = new Ray(center + Vector3.UnitZ * 0.5f, Vector3.UnitZ);

        IntersectionResult result = sphere.Intersect(ray, float.MaxValue);

        Assert.AreEqual(RayIntersection.Inside, result.RayIntersection);
        Assert.AreEqual(1.5f, result.Distance);
        Assert.AreEqual(Vector3.UnitZ, result.Normal);
    }

    [TestMethod]
    public void InsideExitBeyondDistanceLimit_IsMiss()
    {
        var sphere = new Sphere(Vector3.Zero, 2f, new Material(Vector3.One), null);
        float distance = 1f;

        RayIntersection result = sphere.Intersects(new Ray(Vector3.Zero, Vector3.UnitZ), ref distance);

        Assert.AreEqual(RayIntersection.Miss, result);
        Assert.AreEqual(1f, distance);
    }

    [TestMethod]
    public void RayMissesSphere()
    {
        var material = new Material(new Vector3(1,1,1));
        var sphere = new Sphere(new Vector3(0,0,0), 1.0f, material, null);
        var ray = new Ray(new Vector3(0,0,-5), Vector3.Normalize(new Vector3(0,2,1)));

        float dist = float.MaxValue;
        var result = sphere.Intersects(ray, ref dist);
        Assert.AreEqual(RayIntersection.Miss, result);
    }

    [TestMethod]
    public void RayHitsSphere()
    {
        var material = new Material(new Vector3(1,1,1));
        var sphere = new Sphere(new Vector3(0,0,0), 1.0f, material, null);
        var ray = new Ray(new Vector3(0,0,-5), Vector3.Normalize(new Vector3(0,0,1)));

        float dist = float.MaxValue;
        var result = sphere.Intersects(ray, ref dist);
        Assert.IsTrue(result == RayIntersection.Hit || result == RayIntersection.Inside);
        Assert.IsLessThan(float.MaxValue, dist);
    }
}
