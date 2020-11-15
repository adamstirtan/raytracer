using System.Numerics;

using RayTracer.Core.Materials;
using RayTracer.Core.Math;

namespace RayTracer.Core.Primitives
{
    public class Plane : Primitive
    {
        public Vector3 Normal { get; set; }
        public float D { get; set; }

        public Plane(Vector3 normal, float d, Material material)
            : base(material)
        {
            Normal = normal;
            D = d;
        }

        public override Vector3 GetNormal(Vector3 position)
        {
            return Normal;
        }

        public override PrimitiveType GetPrimitiveType()
        {
            return PrimitiveType.Plane;
        }

        public override RayIntersection Intersects(Ray ray, ref float distance)
        {
            float d = Vector3.Dot(Normal, ray.Direction);
            float dist = -(Vector3.Dot(Normal, ray.Origin) + D) / d;

            if (dist > 0)
            {
                if (dist < distance)
                {
                    distance = dist;
                    return RayIntersection.Hit;
                }
            }

            return RayIntersection.Miss;
        }
    }
}