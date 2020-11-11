using System;
using System.Numerics;

using RayTracer.Core.Materials;

namespace RayTracer.Core.Primitives
{
    public class Sphere : Primitive
    {
        public Sphere(Vector3 center, float radius, Material material)
            : base(material)
        {
            Center = center;
            Radius = radius;
        }

        public Vector3 Center { get; set; }

        public float Radius { get; set; }

        public override PrimitiveType GetPrimitiveType()
        {
            return PrimitiveType.Sphere;
        }

        public override IntersectionResult Intersects(Ray ray, float distance)
        {
            var v = ray.Origin - Center;

            float b = -Vector3.Dot(v, ray.Direction);
            float det = (b * b) - Vector3.Dot(v, v) + Radius;

            if (det > 0)
            {
                det = (float)Math.Sqrt(det);
                float i1 = b - det;
                float i2 = b + det;

                if (i2 > 0)
                {
                    if (i1 < 0)
                    {
                        if (i2 < distance)
                        {
                            return new IntersectionResult(RayIntersection.Inside, i2);
                        }
                    }
                    else
                    {
                        if (i1 < distance)
                        {
                            return new IntersectionResult(RayIntersection.Hit, i1);
                        }
                    }
                }
            }

            return new IntersectionResult(RayIntersection.Miss, distance);
        }

        public override Vector3 GetNormal(Vector3 position)
        {
            return (position - Center) * (1.0f / Radius);
        }
    }
}