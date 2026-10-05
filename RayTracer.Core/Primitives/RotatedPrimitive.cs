using System;
using System.Numerics;
using RayTracer.Core.Math;

namespace RayTracer.Core.Primitives;

/// <summary>Places a primitive defined around the origin using a rigid rotation and translation.</summary>
public sealed class RotatedPrimitive : Primitive
{
    private readonly Primitive _primitive;
    private readonly Quaternion _rotation;
    private readonly Quaternion _inverse;
    private readonly Vector3 _position;

    public RotatedPrimitive(Primitive primitive, Quaternion rotation, Vector3 position)
        : base(primitive.Material, primitive.Texture)
    {
        if (rotation.LengthSquared() == 0) throw new ArgumentException("Rotation must be nonzero.", nameof(rotation));
        _primitive = primitive;
        _rotation = Quaternion.Normalize(rotation);
        _inverse = Quaternion.Conjugate(_rotation);
        _position = position;
    }

    public override PrimitiveType GetPrimitiveType() => _primitive.GetPrimitiveType();

    private Vector3 ToLocal(Vector3 position) => Vector3.Transform(position - _position, _inverse);

    public override IntersectionResult Intersect(Ray ray, float maxDistance)
    {
        var localRay = new Ray(ToLocal(ray.Origin), Vector3.Transform(ray.Direction, _inverse));
        IntersectionResult hit = _primitive.Intersect(localRay, maxDistance);
        return new IntersectionResult(hit.RayIntersection, hit.Distance, Vector3.Transform(hit.Normal, _rotation));
    }

    public override RayIntersection Intersects(Ray ray, ref float distance)
    {
        IntersectionResult hit = Intersect(ray, distance);
        distance = hit.Distance;
        return hit.RayIntersection;
    }

    public override Vector3 GetNormal(Vector3 position)
        => Vector3.Transform(_primitive.GetNormal(ToLocal(position)), _rotation);

    public override Vector2 GetUV(Vector3 position) => _primitive.GetUV(ToLocal(position));
}
