namespace RayTracer.Core
{
    public abstract class PrimitiveBase
    {
        public abstract IntersectionResult Intersects(Ray ray, float distance);
    }
}