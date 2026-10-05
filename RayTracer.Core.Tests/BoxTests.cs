using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;
using RayTracer.Core.Math;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Tests;

[TestClass]
public class BoxTests
{
    [TestMethod]
    [DataRow(2f, 7f, 9f, -1f, 0f, 0f)]
    [DataRow(6f, 7f, 9f, 1f, 0f, 0f)]
    [DataRow(4f, 4f, 9f, 0f, -1f, 0f)]
    [DataRow(4f, 10f, 9f, 0f, 1f, 0f)]
    [DataRow(4f, 7f, 6f, 0f, 0f, -1f)]
    [DataRow(4f, 7f, 12f, 0f, 0f, 1f)]
    public void FaceNormals_PointOutward_AtSurfaceAndRayIntersection(
        float x, float y, float z, float nx, float ny, float nz)
    {
        var box = new Box(new Vector3(2, 4, 6), new Vector3(6, 10, 12),
            new Material(Vector3.One), null);
        var position = new Vector3(x, y, z);
        var expectedNormal = new Vector3(nx, ny, nz);

        Assert.AreEqual(expectedNormal, box.GetNormal(position));

        var ray = new Ray(position + expectedNormal * 2f, -expectedNormal);
        IntersectionResult hit = box.Intersect(ray, float.MaxValue);

        Assert.AreEqual(RayIntersection.Hit, hit.RayIntersection);
        Assert.AreEqual(2f, hit.Distance);
        Assert.AreEqual(expectedNormal, hit.Normal);
    }
}
