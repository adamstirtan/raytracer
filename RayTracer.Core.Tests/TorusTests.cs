using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;
using RayTracer.Core.Math;
using Xunit;

namespace RayTracer.Core.Tests
{
    public class TorusTests
    {
        [Fact]
        public void Torus_PrimitiveType_IsTorus()
        {
            var torus = new Torus(new Material(), null, 2.0f, 0.5f);
            Assert.Equal(PrimitiveType.Torus, torus.GetPrimitiveType());
        }

        [Fact]
        public void Torus_RayMissesWhenFar()
        {
            var torus = new Torus(new Material(), null, 2.0f, 0.5f);
            var ray = new Ray(new Vector3(100,100,100), Vector3.Normalize(new Vector3(1,0,0)));
            float dist = float.MaxValue;
            var res = torus.Intersects(ray, ref dist);
            Assert.Equal(RayIntersection.Miss, res);
        }

        [Fact]
        public void Torus_RayHitsWhenAimed()
        {
            var torus = new Torus(new Material(), null, 2.0f, 0.5f);
            // place ray pointing at torus center from front
            var ray = new Ray(new Vector3(0,0, -10), Vector3.Normalize(new Vector3(0,0,1)));
            float dist = 100f;
            var res = torus.Intersects(ray, ref dist);
            Assert.Equal(RayIntersection.Hit, res);
            Assert.True(dist > 0 && dist < 100f);
        }
    }
}
