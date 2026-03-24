using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Math;

namespace RayTracer.Core.Primitives;

public class Torus : Primitive
{
    // Major radius (distance from center to tube center)
    public float MajorRadius { get; set; } = 2.0f;
    // Minor radius (tube radius)
    public float MinorRadius { get; set; } = 0.5f;

    public Torus(Material material, Texture? texture, float major = 2.0f, float minor = 0.5f) : base(material, texture)
    {
        MajorRadius = major;
        MinorRadius = minor;
    }

    public override PrimitiveType GetPrimitiveType() => PrimitiveType.Torus;

    // Signed distance function for torus centered at origin, aligned with Y up:
    // sdf(p) = length(vec2(length(p.xz) - R, p.y)) - r
    private float SDF(Vector3 p)
    {
        var xz = new Vector2(p.X, p.Z);
        float lenXZ = xz.Length();
        var v = new Vector2(lenXZ - MajorRadius, p.Y);
        return v.Length() - MinorRadius;
    }

    public override RayIntersection Intersects(Ray ray, ref float distance)
    {
        // Ray-marching using SDF. Not perfect but simple and robust for tests/scenes.
        const int maxSteps = 100;
        const float hitEps = 1e-3f;
        const float maxDist = 100f;

        float t = 0f;
        for (int i = 0; i < maxSteps && t < distance && t < maxDist; i++)
        {
            var p = ray.Origin + ray.Direction * t;
            float d = SDF(p);
            if (d < hitEps)
            {
                distance = t;
                // estimate normal
                _lastHitNormal = EstimateNormal(p);
                return RayIntersection.Hit;
            }
            t += d;
            if (d < 0) // inside surface, step out
                t += hitEps;
        }
        return RayIntersection.Miss;
    }

    private Vector3 _lastHitNormal = Vector3.UnitY;

    private Vector3 EstimateNormal(Vector3 p)
    {
        // numerical gradient
        float eps = 1e-4f;
        float dx = SDF(new Vector3(p.X + eps, p.Y, p.Z)) - SDF(new Vector3(p.X - eps, p.Y, p.Z));
        float dy = SDF(new Vector3(p.X, p.Y + eps, p.Z)) - SDF(new Vector3(p.X, p.Y - eps, p.Z));
        float dz = SDF(new Vector3(p.X, p.Y, p.Z + eps)) - SDF(new Vector3(p.X, p.Y, p.Z - eps));
        var n = new Vector3(dx, dy, dz);
        if (n == Vector3.Zero) return Vector3.UnitY;
        return Vector3.Normalize(n);
    }

    public override Vector3 GetNormal(Vector3 position)
    {
        return _lastHitNormal;
    }

    public override Vector2 GetUV(Vector3 position)
    {
        // approximate UV: param by torus angles
        // Project to XZ to get angle around major radius, and around tube for minor angle
        var p = position;
        float theta = System.MathF.Atan2(p.Z, p.X); // around Y
        var xz = new Vector2(p.X, p.Z);
        float lenXZ = xz.Length();
        float phi = System.MathF.Atan2(p.Y, lenXZ - MajorRadius);
        // map to 0..1
        return new Vector2((theta + System.MathF.PI) / (2 * System.MathF.PI), (phi + System.MathF.PI) / (2 * System.MathF.PI));
    }
}
