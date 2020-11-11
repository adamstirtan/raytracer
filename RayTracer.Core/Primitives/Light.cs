using System.Numerics;

using RayTracer.Core.Materials;

namespace RayTracer.Core.Primitives
{
    public class Light : Sphere
    {
        public Light(Vector3 center, float radius, Material material)
            : base(center, radius, material)
        { }

        public override PrimitiveType GetPrimitiveType()
        {
            return PrimitiveType.Light;
        }
    }
}