using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Scenes;

namespace RayTracer.Core.Tests;

[TestClass]
public class ImpactSceneTests
{
    [TestMethod]
    public void FrozenSnapshot_PreservesOriginalSimulationPositionsAndVelocities()
    {
        const double time = 3.15;
        var original = new FallingSpheresScene();
        original.Dynamics.Advance(time);
        var impact = new ImpactScene(time);
        Assert.HasCount(original.Dynamics.Bodies.Count, impact.Dynamics.Bodies);
        int rising = 0, falling = 0;
        for (int i = 0; i < original.Dynamics.Bodies.Count; i++)
        {
            var a = original.Dynamics.Bodies[i];
            var b = impact.Dynamics.Bodies[i];
            Assert.AreEqual(a.Sphere.Center, b.Sphere.Center);
            Assert.AreEqual(a.Velocity, b.Velocity);
            Assert.AreEqual(a.Sphere.Radius, b.Sphere.Radius);
            Assert.IsGreaterThanOrEqualTo(b.Sphere.Radius - .002f, b.Sphere.Center.Y);
            if (b.Velocity.Y > .5f) rising++;
            if (b.Velocity.Y < -.5f) falling++;
        }
        Assert.IsGreaterThan(0, rising, "The captured instant needs airborne rebounds.");
        Assert.IsGreaterThan(0, falling, "The captured instant needs continuing downward motion.");
    }
}
