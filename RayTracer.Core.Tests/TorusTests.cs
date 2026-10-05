using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;
using RayTracer.Core.Math;

namespace RayTracer.Core.Tests;

[TestClass]
public class TorusTests
{
    [TestMethod]
    public void InsideRay_ReturnsExit()
    {
        var torus = new Torus(new Material(Vector3.One), null);
        float distance = float.MaxValue;
        Assert.AreEqual(RayIntersection.Inside, torus.Intersects(new Ray(new Vector3(2, 0, 0), Vector3.UnitX), ref distance));
        Assert.AreEqual(0.5f, distance, 0.001f);
    }

    [TestMethod]
    public void DistantRay_HitsBeyondOneHundredUnits()
    {
        var torus = new Torus(new Material(Vector3.One), null);
        var ray = new Ray(new Vector3(0, 0, -150), Vector3.UnitZ);
        float distance = 147f;
        Assert.AreEqual(RayIntersection.Miss, torus.Intersects(ray, ref distance));
        Assert.AreEqual(147f, distance);
        distance = 200f;
        Assert.AreEqual(RayIntersection.Hit, torus.Intersects(ray, ref distance));
        Assert.AreEqual(147.5f, distance, 0.001f);
    }

    [TestMethod]
    public void Torus_PrimitiveType_IsTorus()
    {
        var torus = new Torus(new Material(new System.Numerics.Vector3(0.8f,0.3f,0.2f)), null, 2.0f, 0.5f);
        Assert.AreEqual(PrimitiveType.Torus, torus.GetPrimitiveType());
    }

    [TestMethod]
    public void Torus_RayMissesWhenFar()
    {
        var torus = new Torus(new Material(new System.Numerics.Vector3(0.8f,0.3f,0.2f)), null, 2.0f, 0.5f);
        var ray = new Ray(new Vector3(100,100,100), Vector3.Normalize(new Vector3(1,0,0)));
        float dist = float.MaxValue;
        var res = torus.Intersects(ray, ref dist);
        Assert.AreEqual(RayIntersection.Miss, res);
    }

    [TestMethod]
    public void Torus_RayHitsWhenAimed()
    {
        var torus = new Torus(new Material(new System.Numerics.Vector3(0.8f,0.3f,0.2f)), null, 2.0f, 0.5f);
        // place ray pointing at torus center from front
        var ray = new Ray(new Vector3(0,0, -10), Vector3.Normalize(new Vector3(0,0,1)));
        float dist = 100f;
        var res = torus.Intersects(ray, ref dist);
        Assert.AreEqual(RayIntersection.Hit, res);
        Assert.IsTrue(dist > 0 && dist < 100f);
    }

    [TestMethod]
    public void Torus_RayHitsWhenTranslatedAboveGround()
    {
        var torus = new Torus(new Material(new System.Numerics.Vector3(0.8f,0.3f,0.2f)), null, 2.0f, 0.5f, new Vector3(0, 0.5f, 0));
        var ray = new Ray(new Vector3(0, 0.5f, -10), Vector3.UnitZ);
        float dist = 100f;

        var res = torus.Intersects(ray, ref dist);

        Assert.AreEqual(RayIntersection.Hit, res);
        Assert.IsTrue(dist > 0 && dist < 100f);
    }

    [TestMethod]
    public void Torus_NormalUsesWorldPositionRelativeToCenter()
    {
        var torus = new Torus(new Material(new System.Numerics.Vector3(0.8f,0.3f,0.2f)), null, 2.0f, 0.5f, new Vector3(1, 0.5f, -2));
        var worldPoint = new Vector3(3.5f, 0.5f, -2f);

        var normal = torus.GetNormal(worldPoint);

        Assert.IsTrue(Vector3.Distance(normal, Vector3.UnitX) < 0.01f);
    }
}
