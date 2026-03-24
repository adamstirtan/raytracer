using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;
using RayTracer.Core.Primitives;
using RayTracer.Core.Math;
using RayTracer.Core.Materials;

namespace RayTracer.Core.Tests;

[TestClass]
public class TriangleTests
{
    [TestMethod]
    public void RayHitsTriangle()
    {
        var a = new Vector3(-1, 0, 5);
        var b = new Vector3(1, 0, 5);
        var c = new Vector3(0, 2, 5);

        var tri = new Triangle(a, b, c, new Material(new Vector3(1,0,0)), null);
        var ray = new Ray(new Vector3(0,1,0), Vector3.Normalize(new Vector3(0,0,1)));

        float dist = float.MaxValue;
        var res = tri.Intersects(ray, ref dist);

        Assert.AreEqual(RayIntersection.Hit, res);
        Assert.IsGreaterThan(dist, 0);
        Assert.IsLessThan(dist, float.MaxValue);
    }

    [TestMethod]
    public void RayMissesTriangle()
    {
        var a = new Vector3(-1, 0, 5);
        var b = new Vector3(1, 0, 5);
        var c = new Vector3(0, 2, 5);

        var tri = new Triangle(a, b, c, new Material(new Vector3(1,0,0)), null);
        var ray = new Ray(new Vector3(3,1,0), Vector3.Normalize(new Vector3(0,0,1)));

        float dist = float.MaxValue;
        var res = tri.Intersects(ray, ref dist);

        Assert.AreEqual(RayIntersection.Miss, res);
    }

    [TestMethod]
    public void TriangleNormalIsConsistent()
    {
        var a = new Vector3(-1, 0, 5);
        var b = new Vector3(1, 0, 5);
        var c = new Vector3(0, 2, 5);

        var tri = new Triangle(a, b, c, new Material(new Vector3(1,0,0)), null);

        var normal = tri.GetNormal(new Vector3(0,0,5));
        // Expected normal points roughly towards -Z in this setup? Actually triangle lies in plane z=5, so normal should be +Z
        Assert.IsGreaterThan(Vector3.Dot(normal, Vector3.UnitZ), 0.9f);
    }
}
