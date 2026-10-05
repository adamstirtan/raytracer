using System.Numerics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;
using RayTracer.Core.Math;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Tests;

[TestClass]
public class MeshTests
{
    private static Mesh CreateOverlappingMesh(bool nearFirst)
    {
        var mesh = new Mesh(new Material(Vector3.One), null);
        mesh.Vertices.AddRange(new[]
        {
            new Vector3(-1, -1, 5), new Vector3(1, -1, 5), new Vector3(0, 1, 5),
            new Vector3(-1, -1, 2), new Vector3(0, 1, 2), new Vector3(1, -1, 2)
        });
        mesh.Triangles.AddRange(nearFirst
            ? new[] { (3, 4, 5), (0, 1, 2) }
            : new[] { (0, 1, 2), (3, 4, 5) });
        return mesh;
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void OverlappingTriangles_ReturnNearestDistanceAndNormal_RegardlessOfOrder(bool nearFirst)
    {
        Mesh mesh = CreateOverlappingMesh(nearFirst);
        var ray = new Ray(Vector3.Zero, Vector3.UnitZ);
        IntersectionResult result = mesh.Intersect(ray, float.MaxValue);

        Assert.AreEqual(RayIntersection.Hit, result.RayIntersection);
        Assert.AreEqual(2f, result.Distance);
        Assert.AreEqual(-Vector3.UnitZ, result.Normal);

        float distance = float.MaxValue;
        Assert.AreEqual(RayIntersection.Hit, mesh.Intersects(ray, ref distance));
        Assert.AreEqual(2f, distance);
    }

    [TestMethod]
    public void OverlappingTriangles_RespectMaximumDistance()
    {
        Mesh mesh = CreateOverlappingMesh(false);
        var ray = new Ray(Vector3.Zero, Vector3.UnitZ);

        IntersectionResult hit = mesh.Intersect(ray, 3f);
        Assert.AreEqual(RayIntersection.Hit, hit.RayIntersection);
        Assert.AreEqual(2f, hit.Distance);
        Assert.AreEqual(-Vector3.UnitZ, hit.Normal);

        IntersectionResult miss = mesh.Intersect(ray, 2f);
        Assert.AreEqual(RayIntersection.Miss, miss.RayIntersection);
        Assert.AreEqual(2f, miss.Distance);
    }

    private static Mesh CreateMesh()
    {
        var mesh = new Mesh(new Material(Vector3.One), null);
        mesh.Vertices.AddRange(new[]
        {
            new Vector3(-1, -1, 2), new Vector3(1, -1, 2), new Vector3(0, 1, 2),
            new Vector3(3, -1, 1), new Vector3(3, 1, 1), new Vector3(3, 0, 3)
        });
        mesh.Triangles.AddRange(new[] { (0, 1, 2), (3, 4, 5) });
        return mesh;
    }

    [TestMethod]
    public void InterleavedHits_PreserveEachRaysNormal()
    {
        Mesh mesh = CreateMesh();
        IntersectionResult first = mesh.Intersect(new Ray(Vector3.Zero, Vector3.UnitZ), float.MaxValue);
        IntersectionResult second = mesh.Intersect(new Ray(new Vector3(5, 0, 2), -Vector3.UnitX), float.MaxValue);

        Assert.AreEqual(RayIntersection.Hit, first.RayIntersection);
        Assert.AreEqual(2f, first.Distance);
        Assert.AreEqual(Vector3.UnitZ, first.Normal);
        Assert.AreEqual(RayIntersection.Hit, second.RayIntersection);
        Assert.AreEqual(2f, second.Distance);
        Assert.AreEqual(Vector3.UnitX, second.Normal);
        Assert.AreEqual(Vector3.UnitZ, mesh.GetNormal(new Vector3(0, 0, 2)));
        Assert.AreEqual(Vector3.UnitX, mesh.GetNormal(new Vector3(3, 0, 2)));
    }

    [TestMethod]
    public void ConcurrentHits_ReturnNormalsForTheirOwnFaces()
    {
        Mesh mesh = CreateMesh();
        var results = new IntersectionResult[1000];
        Parallel.For(0, results.Length, i =>
        {
            Ray ray = i % 2 == 0
                ? new Ray(Vector3.Zero, Vector3.UnitZ)
                : new Ray(new Vector3(5, 0, 2), -Vector3.UnitX);
            results[i] = mesh.Intersect(ray, float.MaxValue);
        });

        for (int i = 0; i < results.Length; i++)
        {
            Assert.AreEqual(RayIntersection.Hit, results[i].RayIntersection);
            Assert.AreEqual(i % 2 == 0 ? Vector3.UnitZ : Vector3.UnitX, results[i].Normal);
        }
    }

    [TestMethod]
    public void LegacyIntersection_PreservesDistanceLimit()
    {
        Mesh mesh = CreateMesh();
        float distance = 1f;
        RayIntersection result = mesh.Intersects(new Ray(Vector3.Zero, Vector3.UnitZ), ref distance);

        Assert.AreEqual(RayIntersection.Miss, result);
        Assert.AreEqual(1f, distance);
    }
}
