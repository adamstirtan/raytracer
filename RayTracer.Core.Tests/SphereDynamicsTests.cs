using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;
using RayTracer.Core.Physics;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Tests;

[TestClass]
public class SphereDynamicsTests
{
    private static SphereDynamics.Body Body(Vector3 position, Vector3 velocity)
        => new(new Sphere(position,.5f,new Material(Vector3.One),null)){Velocity=velocity};

    [TestMethod]
    public void FreeFall_UsesEarthGravityAndIsIndependentOfFramePartition()
    {
        var a=Body(new(0,20,0),Vector3.Zero); var b=Body(new(0,20,0),Vector3.Zero);
        var first=new SphereDynamics(new[]{a}); var second=new SphereDynamics(new[]{b});
        first.Advance(1); for(int i=0;i<30;i++) second.Advance(1.0/30);
        Assert.AreEqual(-SphereDynamics.Gravity,a.Velocity.Y,.001f);
        Assert.AreEqual(20-SphereDynamics.Gravity/2,a.Sphere.Center.Y,.025f);
        Assert.IsTrue(Vector3.Distance(a.Sphere.Center,b.Sphere.Center)<.00001f);
    }

    [TestMethod]
    public void Floor_BouncesUpwardWithoutPenetration()
    {
        var body=Body(new(0,.51f,0),new(0,-4,0));
        new SphereDynamics(new[]{body}).Advance(.01);
        Assert.IsTrue(body.Velocity.Y>0);
        Assert.IsTrue(body.Sphere.Center.Y>=body.Sphere.Radius);
    }

    [TestMethod]
    public void EqualMassCollision_ReversesDirectionsAndConservesHorizontalMomentum()
    {
        var a=Body(new(-.51f,5,0),new(2,0,0)); var b=Body(new(.51f,5,0),new(-2,0,0));
        new SphereDynamics(new[]{a,b}){Restitution=1,Friction=0}.Advance(.03);
        Assert.IsTrue(a.Velocity.X<0 && b.Velocity.X>0);
        Assert.AreEqual(0,a.Velocity.X+b.Velocity.X,.0001f);
        Assert.AreEqual(2,b.Velocity.X,.001f);
        Assert.IsTrue(Vector3.Distance(a.Sphere.Center,b.Sphere.Center)>=.999f);
    }
}
