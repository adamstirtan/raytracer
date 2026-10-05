using System;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;
using RayTracer.Core.Math;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Tests;

[TestClass]
public class RotatedPrimitiveTests
{
    [TestMethod]
    public void RotatedDisk_IntersectsInWorldSpaceAndReturnsWorldNormal()
    {
        var disk = new Disk(Vector3.Zero, Vector3.UnitY, 2, new Material(Vector3.One), null);
        Quaternion rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2);
        var transformed = new RotatedPrimitive(disk, rotation, new Vector3(3, 4, 5));
        var result = transformed.Intersect(new Ray(new Vector3(3, 4, 1), Vector3.UnitZ), 10);
        Assert.AreEqual(RayIntersection.Hit, result.RayIntersection);
        Assert.AreEqual(4f, result.Distance, 1e-5f);
        Assert.IsLessThan(1e-5f, Vector3.Distance(Vector3.UnitZ, result.Normal));
        Assert.IsLessThan(1e-5f, Vector2.Distance(new Vector2(0.5f), transformed.GetUV(new Vector3(3, 4, 5))));
        float distance = 3;
        Assert.AreEqual(RayIntersection.Miss, transformed.Intersects(new Ray(new Vector3(3, 4, 1), Vector3.UnitZ), ref distance));
        Assert.AreEqual(3f, distance);
    }
}
