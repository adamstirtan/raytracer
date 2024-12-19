namespace RayTracer.Core.Math;

public class IntersectionResult(RayIntersection rayIntersection, float distance)
{
    public RayIntersection RayIntersection { get; set; } = rayIntersection;

    public float Distance { get; set; } = distance;
}

public enum RayIntersection
{
    Hit,
    Miss,
    Inside
}