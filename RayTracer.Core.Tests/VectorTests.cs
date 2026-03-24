using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RayTracer.Core.Tests;

[TestClass]
public class VectorTests
{
    [TestMethod]
    public void VectorAdditionAndDot()
    {
        var a = new Vector3(1, 2, 3);
        var b = new Vector3(4, -1, 2);

        var sum = a + b;
        Assert.AreEqual(new Vector3(5, 1, 5), sum);

        var dot = Vector3.Dot(a, b);
        Assert.AreEqual(1*4 + 2*(-1) + 3*2, dot);
    }

    [TestMethod]
    public void VectorNormalization()
    {
        var v = new Vector3(3, 0, 4);
        var norm = Vector3.Normalize(v);
        // length should be 1
        var len = norm.Length();
        Assert.IsLessThan(System.Math.Abs(len - 1.0f), 1e-6f);
    }
}
