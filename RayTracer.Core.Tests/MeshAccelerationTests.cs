using System;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;
using RayTracer.Core.Math;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Tests;

[TestClass]
public class MeshAccelerationTests
{
    private static Mesh Geometry()
    {
        var mesh = new Mesh(new Material(Vector3.One), null);
        var random = new Random(763);
        for (int i = 0; i < 200; i++)
        {
            Vector3 center = new((float)random.NextDouble() * 12 - 6, (float)random.NextDouble() * 12 - 6,
                2 + (float)random.NextDouble() * 10);
            int start = mesh.Vertices.Count;
            mesh.Vertices.Add(center + new Vector3(-.9f, -.7f, 0));
            mesh.Vertices.Add(center + new Vector3(.9f, -.7f, .2f));
            mesh.Vertices.Add(center + new Vector3(0, .9f, -.1f));
            mesh.Triangles.Add((start, start + 1, start + 2));
        }
        return mesh;
    }

    [TestMethod]
    public void BvhMatchesBruteForce_WithConcurrentRaysAndDistanceLimits()
    {
        Mesh brute = Geometry(), fast = Geometry();
        fast.BuildAcceleration();
        Parallel.For(0, 1200, i =>
        {
            float x = i % 30 / 29f * 14 - 7, y = i / 30 % 20 / 19f * 14 - 7;
            Vector3 direction = i % 3 == 0 ? -Vector3.UnitZ : i % 2 == 0 ? Vector3.UnitZ : new(.05f, -.04f, 2);
            var ray = new Ray(new(x, y, i % 3 == 0 ? 7 : 0), direction);
            float limit = i % 4 == 0 ? 5 : float.MaxValue;
            var expected = brute.Intersect(ray, limit);
            var actual = fast.Intersect(ray, limit);
            Assert.AreEqual(expected.RayIntersection, actual.RayIntersection);
            Assert.AreEqual(expected.Distance, actual.Distance);
            Assert.AreEqual(expected.Normal, actual.Normal);
        });
    }

    [TestMethod]
    public void RebuildingUsesEditedGeometry_AndHitAtDistanceLimitIsExcluded()
    {
        var mesh = new Mesh(new Material(Vector3.One), null);
        mesh.Vertices.AddRange(new[] { new Vector3(-1,-1,2), new Vector3(1,-1,2), new Vector3(0,1,2) });
        mesh.Triangles.Add((0,1,2));
        mesh.BuildAcceleration();
        var ray = new Ray(Vector3.Zero, Vector3.UnitZ);
        Assert.AreEqual(RayIntersection.Miss, mesh.Intersect(ray, 2).RayIntersection);
        for (int i = 0; i < 3; i++) mesh.Vertices[i] += Vector3.UnitZ * 3;
        mesh.BuildAcceleration();
        Assert.AreEqual(5f, mesh.Intersect(ray, 6).Distance);
        Assert.AreEqual(RayIntersection.Miss, mesh.Intersect(new Ray(new(2,0,0), Vector3.UnitZ), 10).RayIntersection);
    }

    [TestMethod]
    public void InvalidAndZeroDirections_DoNotProduceFalseHits()
    {
        Mesh mesh = Geometry();
        mesh.BuildAcceleration();
        foreach (var direction in new[] { Vector3.Zero, new Vector3(float.NaN, 0, 1) })
        {
            var result = mesh.Intersect(new Ray(new(0,0,7), direction), 20);
            Assert.AreEqual(RayIntersection.Miss, result.RayIntersection);
            Assert.AreEqual(20f, result.Distance);
        }
    }

    [TestMethod]
    public void SmoothShadingInterpolatesNormals_WithoutChangingIntersectionDistance()
    {
        var mesh = new Mesh(new Material(Vector3.One), null);
        mesh.Vertices.AddRange(new[] { new Vector3(-1,-1,2), new Vector3(1,-1,2), new Vector3(0,1,2) });
        mesh.Triangles.Add((0,1,2));
        Vector3 n0 = Vector3.Normalize(new Vector3(-.3f,0,1)), n1 = Vector3.Normalize(new Vector3(.3f,0,1));
        mesh.VertexNormals.AddRange(new[] { n0, n1, Vector3.UnitZ });
        mesh.BuildAcceleration(smoothShading: true);
        var hit = mesh.Intersect(new Ray(new(-.5f,-.5f,0), Vector3.UnitZ), 10);
        Assert.AreEqual(2f, hit.Distance);
        Vector3 expected = Vector3.Normalize(n0 * .625f + n1 * .125f + Vector3.UnitZ * .25f);
        Assert.IsLessThan(.00001f, Vector3.Distance(expected, hit.Normal));
    }
}
