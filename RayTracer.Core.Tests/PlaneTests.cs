using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Tests
{
    [TestClass]
    public class PlaneTests
    {
        [TestMethod]
        public void GetUV_ReturnsValuesInUnitRange_ForVariousPositions()
        {
            var plane = new RayTracer.Core.Primitives.Plane(new Vector3(0, 1, 0), 0, null, null);
            var positions = new[]
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(1.25f, 0f, -0.75f),
                new Vector3(-5.3f, 0f, 7.8f),
                new Vector3(10.0f, 0f, 10.0f)
            };

            foreach (var p in positions)
            {
                var uv = plane.GetUV(p);
                Assert.IsTrue(uv.X >= 0f && uv.X < 1f, $"U out of range: {uv.X} for position {p}");
                Assert.IsTrue(uv.Y >= 0f && uv.Y < 1f, $"V out of range: {uv.Y} for position {p}");
            }
        }
    }
}
