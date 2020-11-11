using System.Numerics;

using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core
{
    public abstract class Primitive
    {
        public Material Material { get; set; }

        protected Primitive(Material material)
        {
            Material = material;
        }

        public abstract PrimitiveType GetPrimitiveType();

        public abstract IntersectionResult Intersects(Ray ray, float distance);

        public abstract Vector3 GetNormal(Vector3 position);
    }
}