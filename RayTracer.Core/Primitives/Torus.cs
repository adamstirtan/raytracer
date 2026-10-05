using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Math;

namespace RayTracer.Core.Primitives;

public class Torus : Primitive
{
    public Vector3 Center { get; set; }

    // Major radius (distance from center to tube center)
    public float MajorRadius { get; set; } = 2.0f;
    // Minor radius (tube radius)
    public float MinorRadius { get; set; } = 0.5f;

    public Torus(Material material, Texture? texture, float major = 2.0f, float minor = 0.5f, Vector3 center = default) : base(material, texture)
    {
        Center = center;
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
        const int maxSteps = 512;
        const float hitEps = 5e-4f;
        Vector3 origin = ray.Origin - Center;
        float speed = ray.Direction.Length();
        if (speed == 0f) return RayIntersection.Miss;

        // Restrict marching to the bounding sphere, rather than an arbitrary world range.
        float bound = MajorRadius + MinorRadius + hitEps;
        float a = Vector3.Dot(ray.Direction, ray.Direction);
        float b = Vector3.Dot(origin, ray.Direction);
        float c = Vector3.Dot(origin, origin) - bound * bound;
        float discriminant = b * b - a * c;
        if (discriminant < 0f) return RayIntersection.Miss;
        float root = System.MathF.Sqrt(discriminant);
        float end = System.MathF.Min(distance, (-b + root) / a);
        float t = System.MathF.Max(1e-6f, (-b - root) / a);
        bool inside = SDF(origin) < 0f;

        for (int i = 0; i < maxSteps && t < distance && t <= end; i++)
        {
            float d = SDF(origin + ray.Direction * t);
            if (System.MathF.Abs(d) < hitEps)
            {
                distance = t;
                return inside ? RayIntersection.Inside : RayIntersection.Hit;
            }
            t += System.MathF.Abs(d) / speed;
        }
        return RayIntersection.Miss;
    }

    private Vector3 EstimateNormal(Vector3 p)
    {
        // numerical gradient
        float eps = 5e-5f;
        float dx = SDF(new Vector3(p.X + eps, p.Y, p.Z)) - SDF(new Vector3(p.X - eps, p.Y, p.Z));
        float dy = SDF(new Vector3(p.X, p.Y + eps, p.Z)) - SDF(new Vector3(p.X, p.Y - eps, p.Z));
        float dz = SDF(new Vector3(p.X, p.Y, p.Z + eps)) - SDF(new Vector3(p.X, p.Y, p.Z - eps));
        var n = new Vector3(dx, dy, dz);
        if (n == Vector3.Zero) return Vector3.UnitY;
        return Vector3.Normalize(n);
    }

    public override Vector3 GetNormal(Vector3 position)
    {
        return EstimateNormal(position - Center);
    }

    public override Vector2 GetUV(Vector3 position)
    {
        // approximate UV: param by torus angles
        // Project to XZ to get angle around major radius, and around tube for minor angle
        var p = position - Center;
        float theta = System.MathF.Atan2(p.Z, p.X); // around Y
        var xz = new Vector2(p.X, p.Z);
        float lenXZ = xz.Length();
        float phi = System.MathF.Atan2(p.Y, lenXZ - MajorRadius);
        // map to 0..1
        return new Vector2((theta + System.MathF.PI) / (2 * System.MathF.PI), (phi + System.MathF.PI) / (2 * System.MathF.PI));
    }
}
