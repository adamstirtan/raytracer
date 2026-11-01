using System;
using System.Numerics;
using RayTracer.Core.Math;

namespace RayTracer.Core.Primitives;

/// <summary>
/// Wraps another primitive and replaces only its UV mapping. Intersection,
/// shading and normals are delegated unchanged to the inner primitive, so a
/// JSON scene can reproduce the custom GetUV overrides that some code scenes
/// use (a scaled bed, a facade, a mapped sky plane, a spherical asteroid)
/// without needing a new primitive class per scene.
/// Supported modes (see docs/scene-format.md):
/// <list type="bullet">
/// <item><c>xzScale</c>: uv = (position.X * scaleX, position.Z * scaleZ).</item>
/// <item><c>facade</c>: face-based box UV divided by <c>repeat</c> (default 4).</item>
/// <item><c>planeMapped</c>: uv = ((x + offsetX) / width, (offsetY - y) / height),
/// optionally wrapped instead of clamped.</item>
/// <item><c>spherical</c>: uv from the direction of (position - center) / size.</item>
/// </list>
/// </summary>
public sealed class UvMappedPrimitive : Primitive
{
    private readonly Primitive _inner;
    private readonly string _mode;
    private readonly float _scaleX;
    private readonly float _scaleZ;
    private readonly float _repeat;
    private readonly float _offsetX;
    private readonly float _width;
    private readonly float _offsetY;
    private readonly float _height;
    private readonly bool _wrap;
    private readonly Vector3 _center;
    private readonly float _size;

    public UvMappedPrimitive(
        Primitive inner,
        string mode,
        float scaleX = 1,
        float scaleZ = 1,
        float repeat = 4,
        float offsetX = 0,
        float width = 1,
        float offsetY = 0,
        float height = 1,
        bool wrap = false,
        Vector3 center = default,
        float size = 1)
        : base(inner.Material, inner.Texture)
    {
        _inner = inner;
        _mode = mode;
        _scaleX = scaleX;
        _scaleZ = scaleZ;
        _repeat = repeat;
        _offsetX = offsetX;
        _width = width;
        _offsetY = offsetY;
        _height = height;
        _wrap = wrap;
        _center = center;
        _size = size;
    }

    public Primitive Inner => _inner;
    public string Mode => _mode;

    public override PrimitiveType GetPrimitiveType() => _inner.GetPrimitiveType();

    public override IntersectionResult Intersect(Ray ray, float maxDistance)
        => _inner.Intersect(ray, maxDistance);

    public override RayIntersection Intersects(Ray ray, ref float distance)
        => _inner.Intersects(ray, ref distance);

    public override Vector3 GetNormal(Vector3 position) => _inner.GetNormal(position);

    public override Vector2 GetUV(Vector3 position)
    {
        switch (_mode)
        {
            case "xzScale":
                return new Vector2(position.X * _scaleX, position.Z * _scaleZ);
            case "facade":
            {
                if (_inner is not Box box) return _inner.GetUV(position);
                Vector3 p = position - box.Min;
                Vector3 normal = box.GetNormal(position);
                if (MathF.Abs(normal.Y) > .5f) return new Vector2(p.X, p.Z) / _repeat;
                if (MathF.Abs(normal.X) > .5f) return new Vector2(p.Z, -p.Y) / _repeat;
                return new Vector2(p.X, -p.Y) / _repeat;
            }
            case "planeMapped":
            {
                float u = (position.X + _offsetX) / _width;
                float v = (_offsetY - position.Y) / _height;
                if (_wrap) return new Vector2(u - MathF.Floor(u), v - MathF.Floor(v));
                return new Vector2(u, System.Math.Clamp(v, .001f, .999f));
            }
            case "spherical":
            {
                Vector3 p = Vector3.Normalize((position - _center) / _size);
                return new Vector2(.5f + MathF.Atan2(p.Z, p.X) / MathF.Tau,
                    .5f - MathF.Asin(p.Y) / MathF.PI);
            }
            default:
                return _inner.GetUV(position);
        }
    }
}
