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
    [DataRow(0f, 3f, 0f, 0f, -1f, 0f, 2f, 0f, 1f, 0f)]
    [DataRow(0f, -3f, 0f, 0f, 1f, 0f, 2f, 0f, -1f, 0f)]
    [DataRow(0f, 3f, -2f, 0f, -1f, 1f, 2.8284271f, 0f, 1f, 0f)]
    [DataRow(0f, 3f, 0f, 0f, -1f, 0.25f, 2.0615528f, 0f, 1f, 0f)]
    [DataRow(0f, 0f, -3f, 0f, 0f, 1f, 2f, 0f, 0f, -1f)]
    [DataRow(0f, 0f, 0f, 0f, 0f, 1f, 1f, 0f, 0f, 1f)]
    [DataRow(0f, 0f, 0f, 0f, 1f, 0f, 1f, 0f, 1f, 0f)]
    [DataRow(0f, 0f, 0f, 0f, -1f, 0f, 1f, 0f, -1f, 0f)]
    public void Intersection_SelectsNearestValidSurface(
        float ox, float oy, float oz, float dx, float dy, float dz,
        float expectedDistance, float nx, float ny, float nz)
    {
        // Translate the cylinder to also exercise its local-space calculations.
        var center = new Vector3(2, 4, 6);
        var cylinder = new Cylinder(center, 1f, 2f, new Material(Vector3.One), null);
        var ray = new Ray(center + new Vector3(ox, oy, oz),
            Vector3.Normalize(new Vector3(dx, dy, dz)));

        IntersectionResult hit = cylinder.Intersect(ray, float.MaxValue);

        Assert.AreEqual(RayIntersection.Hit, hit.RayIntersection);
        Assert.AreEqual(expectedDistance, hit.Distance, 1e-5f);
        Assert.AreEqual(new Vector3(nx, ny, nz), hit.Normal);
    }

    [TestMethod]
    [DataRow(0f, 3f, 0f, 0f, -1f, 0f, 1.5f)]
    [DataRow(0f, 3f, 0f, 0f, -1f, 0f, 2f)]
    [DataRow(2f, 3f, 0f, 0f, -1f, 0f, 100f)]
    [DataRow(0f, 2f, -3f, 0f, 0f, 1f, 100f)]
    public void Miss_PreservesDistanceLimit(
        float ox, float oy, float oz, float dx, float dy, float dz, float limit)
    {
        var cylinder = new Cylinder(Vector3.Zero, 1f, 2f, new Material(Vector3.One), null);
        float distance = limit;
        RayIntersection result = cylinder.Intersects(
            new Ray(new Vector3(ox, oy, oz), new Vector3(dx, dy, dz)), ref distance);

        Assert.AreEqual(RayIntersection.Miss, result);
        Assert.AreEqual(limit, distance);
    }

    [TestMethod]
    public void RayHitsCylinderSide()
    {
        var cyl = new Cylinder(new Vector3(0,1,5), 1f, 2f, new Material(new Vector3(1,0,0)), null);
        var ray = new Ray(new Vector3(0,1,0), Vector3.Normalize(new Vector3(0,0,1)));
        float dist = float.MaxValue;
        var res = cyl.Intersects(ray, ref dist);
        Assert.AreEqual(RayIntersection.Hit, res);
        Assert.IsGreaterThan(0, dist);
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
