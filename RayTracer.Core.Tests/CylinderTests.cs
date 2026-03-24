using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;
using RayTracer.Core.Primitives;
using RayTracer.Core.Math;
using RayTracer.Core.Materials;

namespace RayTracer.Core.Tests;

[TestClass]
public class CylinderTests
{
    [TestMethod]
    public void RayHitsCylinderSide()
    {
        var cyl = new Cylinder(new Vector3(0,1,5), 1f, 2f, new Material(new Vector3(1,0,0)), null);
        var ray = new Ray(new Vector3(0,1,0), Vector3.Normalize(new Vector3(0,0,1)));
        float dist = float.MaxValue;
        var res = cyl.Intersects(ray, ref dist);
        Assert.AreEqual(RayIntersection.Hit, res);
        Assert.IsGreaterThan(dist, 0);
    }

    [TestMethod]
    public void RayMissesCylinder()
    {
        var cyl = new Cylinder(new Vector3(0,1,5), 1f, 2f, new Material(new Vector3(1,0,0)), null);
        var ray = new Ray(new Vector3(2.5f,1,0), Vector3.Normalize(new Vector3(0,0,1)));
        float dist = float.MaxValue;
        var res = cyl.Intersects(ray, ref dist);
        Assert.AreEqual(RayIntersection.Miss, res);
    }
}
