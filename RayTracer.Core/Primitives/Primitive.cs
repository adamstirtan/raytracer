using System.Numerics;

using RayTracer.Core.Materials;

namespace RayTracer.Core
{
    public abstract class Primitive
    {
        public Material Material { get; set; }

        public abstract IntersectionResult Intersects(Ray ray, float distance);

        public abstract Vector3 GetNormal(Vector3 position);
    }
}