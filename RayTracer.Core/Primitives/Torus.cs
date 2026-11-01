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
        // Analytic torus intersection: the torus (axis Y, ring in the XZ plane)
        // is the quartic (|p|^2 + R^2 - r^2)^2 - 4 R^2 (x^2 + z^2) = 0.
        // Substituting p(t) = origin + t * direction gives a quartic in t.
        // Roots are found by scanning the bounding-sphere interval for sign
        // changes and bisecting them. This replaces the previous SDF sphere
        // tracing, which failed for rays already inside the tube (exactly the
        // refracted rays a glass torus produces): marching by |SDF| inside
        // the tube converges to the wrong surface or stalls, so glass tori
        // rendered black. The analytic form gives the exact first hit, and
        // whether it is an entry (Hit) or an exit (Inside) follows from
        // whether the ray origin is inside the tube.
        Vector3 origin = ray.Origin - Center;
        Vector3 dir = ray.Direction;
        if (dir == Vector3.Zero) return RayIntersection.Miss;

        float r = MinorRadius;
        float bigR = MajorRadius;

        // Quartic coefficients for f(t) = c4 t^4 + c3 t^3 + c2 t^2 + c1 t + c0.
        float a = Vector3.Dot(dir, dir);
        float b = 2f * Vector3.Dot(origin, dir);
        float g = Vector3.Dot(origin, origin) + bigR * bigR - r * r;
        float radial0 = origin.X * origin.X + origin.Z * origin.Z;
        float radial1 = 2f * (origin.X * dir.X + origin.Z * dir.Z);
        float radial2 = dir.X * dir.X + dir.Z * dir.Z;
        float c4 = a * a;
        float c3 = 2f * a * b;
        float c2 = b * b + 2f * a * g - 4f * bigR * bigR * radial2;
        float c1 = 2f * b * g - 4f * bigR * bigR * radial1;
        float c0 = g * g - 4f * bigR * bigR * radial0;

        float F(float t) => ((c4 * t + c3) * t + c2) * t * t + c1 * t + c0;

        // Restrict the search to the bounding sphere, and to the caller's
        // maximum distance (a hit beyond it is a miss, and distance is
        // left unchanged, matching the other primitives).
        float bound = bigR + r;
        float qa = a;
        float qb = 2f * Vector3.Dot(origin, dir);
        float qc = Vector3.Dot(origin, origin) - bound * bound;
        float qdisc = qb * qb - 4f * qa * qc;
        if (qdisc < 0f) return RayIntersection.Miss;
        float qroot = System.MathF.Sqrt(qdisc);
        // Secondary rays are offset 0.001 from the surface they left, so a
        // scan starting at 1e-4 could re-detect that same surface (its
        // implicit value is still near zero a step or two out, especially
        // for shadow rays leaving at a grazing angle) and report a spurious
        // self-hit. That showed up as single-pixel speckle: a shadow ray
        // that "hit" its own torus shadowed that pixel. Start the scan
        // 0.02 out instead; the tube is 0.4 in radius, so a genuine exit
        // for a ray starting inside is never that close, and primary rays
        // start at the bounding-sphere entry, which is farther still.
        float tStart = System.MathF.Max(0.02f, (-qb - qroot) / (2f * qa));
        float tEnd = System.MathF.Min(distance, (-qb + qroot) / (2f * qa));
        if (tStart >= tEnd) return RayIntersection.Miss;

        // Scan for sign changes. Steps are dense enough that a ray crossing
        // the tube cannot jump over it: at least 512 steps, and a step no
        // larger than half the tube radius.
        int steps = (int)System.MathF.Max(512f, System.MathF.Ceiling((tEnd - tStart) / (r * 0.5f)));
        float step = (tEnd - tStart) / steps;
        float tPrev = tStart;
        float fPrev = F(tPrev);
        bool inside = SDF(origin) < 0f;

        for (int i = 1; i <= steps; i++)
        {
            float t = i == steps ? tEnd : tStart + i * step;
            float f = F(t);
            if (System.MathF.Abs(f) < 1e-4f || fPrev * f < 0f)
            {
                // Bisect the sign change (or accept the near-zero sample).
                float lo = tPrev, hi = t, fLo = fPrev;
                if (System.MathF.Abs(f) >= 1e-4f)
                {
                    for (int j = 0; j < 60; j++)
                    {
                        float mid = 0.5f * (lo + hi);
                        float fMid = F(mid);
                        if (fLo * fMid <= 0f) hi = mid;
                        else { lo = mid; fLo = fMid; }
                    }
                }
                else
                {
                    hi = t;
                }
                if (hi > 1e-4f && hi < distance)
                {
                    distance = hi;
                    return inside ? RayIntersection.Inside : RayIntersection.Hit;
                }
                return RayIntersection.Miss;
            }
            tPrev = t;
            fPrev = f;
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
