using System.Numerics;

namespace RayTracer.Core.Math;

public readonly struct IntersectionResult(RayIntersection rayIntersection, float distance, Vector3 normal = default)
{
    public RayIntersection RayIntersection { get; } = rayIntersection;

    public float Distance { get; } = distance;

    public Vector3 Normal { get; } = normal;
}

public enum RayIntersection
{
    Hit,
    Miss,
    Inside
}
