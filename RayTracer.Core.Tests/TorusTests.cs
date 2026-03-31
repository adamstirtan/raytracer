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
}
