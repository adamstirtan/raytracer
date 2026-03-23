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
        Assert.IsTrue(dist < float.MaxValue);
    }
}
