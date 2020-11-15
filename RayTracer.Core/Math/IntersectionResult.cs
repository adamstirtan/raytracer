namespace RayTracer.Core.Math
{
    public class IntersectionResult
    {
        public RayIntersection RayIntersection { get; set; }

        public float Distance { get; set; }

        public IntersectionResult(RayIntersection rayIntersection, float distance)
        {
            RayIntersection = rayIntersection;
            Distance = distance;
        }
    }

    public enum RayIntersection
    {
        Hit,
        Miss,
        Inside
    }
}