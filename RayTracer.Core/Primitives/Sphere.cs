using System.Numerics;

using RayTracer.Core.Materials;
using RayTracer.Core.Math;

namespace RayTracer.Core.Primitives
{
    public class Sphere : Primitive
    {
        private readonly float _radiusSquared;

        public Vector3 Center { get; set; }
        public float Radius { get; set; }

        public Sphere(Vector3 center, float radius, Material material)
            : base(material)
        {
            Center = center;
            Radius = radius;

            _radiusSquared = (float)System.Math.Pow(Radius, 2);
        }

        public override PrimitiveType GetPrimitiveType()
        {
            return PrimitiveType.Sphere;
        }

        public override RayIntersection Intersects(Ray ray, ref float distance)
        {
            var v = ray.Origin - Center;

            float b = -Vector3.Dot(v, ray.Direction);
            float det = (b * b) - Vector3.Dot(v, v) + _radiusSquared;

            if (det > 0)
            {
                det = (float)System.Math.Sqrt(det);
                float i1 = b - det;
                float i2 = b + det;

                if (i2 > 0)
                {
                    if (i1 < 0)
                    {
                        if (i2 < distance)
                        {
                            distance = i2;
                            return RayIntersection.Inside;
                        }
                    }
                    else
                    {
                        if (i1 < distance)
                        {
                            distance = i1;
                            return RayIntersection.Hit;
                        }
                    }
                }
            }

            return RayIntersection.Miss;
        }

        public override Vector3 GetNormal(Vector3 position)
        {
            return (position - Center) * (1.0f / Radius);
        }
    }
}