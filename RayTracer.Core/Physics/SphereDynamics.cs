using System;
using System.Collections.Generic;
using System.Numerics;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Physics;

/// <summary>Fixed-step solid-sphere dynamics. Units are metres, kilograms, and seconds.</summary>
public sealed class SphereDynamics
{
    public sealed class Body(Sphere sphere, float density = 1200)
    {
        public Sphere Sphere { get; } = sphere;
        public Vector3 Velocity { get; set; }
        public Vector3 AngularVelocity { get; set; }
        public float InverseMass { get; } = 1 / (density * 4 / 3 * MathF.PI * sphere.Radius * sphere.Radius * sphere.Radius);
        public float InverseInertia => 2.5f * InverseMass / (Sphere.Radius * Sphere.Radius);
    }

    public const float Gravity = 9.80665f;
    public const double TimeStep = 1.0 / 240;
    public IReadOnlyList<Body> Bodies { get; }
    public float Restitution { get; set; } = .62f;
    public float Friction { get; set; } = .32f;
    private double accumulator;

    public SphereDynamics(IReadOnlyList<Body> bodies) => Bodies = bodies;

    public void Advance(double seconds)
    {
        if (!double.IsFinite(seconds) || seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));
        accumulator += seconds;
        while (accumulator + 1e-12 >= TimeStep)
        {
            Step((float)TimeStep);
            accumulator -= TimeStep;
        }
    }

    private void Step(float dt)
    {
        foreach (var body in Bodies)
        {
            body.Velocity -= Vector3.UnitY * Gravity * dt;
            body.Sphere.Center += body.Velocity * dt;
            // Small rolling resistance dissipates spin without artificial horizontal drag in flight.
            if (body.Sphere.Center.Y <= body.Sphere.Radius + .002f)
                body.AngularVelocity *= MathF.Exp(-.18f * dt);
        }
        for (int iteration = 0; iteration < 6; iteration++)
        {
            foreach (var body in Bodies) Floor(body);
            for (int i = 0; i < Bodies.Count; i++)
                for (int j = i + 1; j < Bodies.Count; j++) Collide(Bodies[i], Bodies[j]);
        }
    }

    private void Floor(Body body)
    {
        float penetration = body.Sphere.Radius - body.Sphere.Center.Y;
        if (penetration < 0) return;
        body.Sphere.Center += Vector3.UnitY * penetration;
        float speed = body.Velocity.Y;
        if (speed >= 0) return;
        float bounce = speed < -.5f ? Restitution : 0;
        float impulse = -(1 + bounce) * speed / body.InverseMass;
        body.Velocity += Vector3.UnitY * impulse * body.InverseMass;
        Vector3 arm = -Vector3.UnitY * body.Sphere.Radius;
        Vector3 contactVelocity = body.Velocity + Vector3.Cross(body.AngularVelocity, arm);
        Vector3 tangent = contactVelocity - Vector3.UnitY * contactVelocity.Y;
        float length = tangent.Length();
        if (length < 1e-7f) return;
        tangent /= length;
        float denominator = body.InverseMass + body.InverseInertia * Vector3.Cross(arm, tangent).LengthSquared();
        Vector3 friction = -tangent * MathF.Min(length / denominator, Friction * impulse);
        body.Velocity += friction * body.InverseMass;
        body.AngularVelocity += Vector3.Cross(arm, friction) * body.InverseInertia;
    }

    private void Collide(Body a, Body b)
    {
        Vector3 delta = b.Sphere.Center - a.Sphere.Center;
        float radii = a.Sphere.Radius + b.Sphere.Radius;
        float squared = delta.LengthSquared();
        if (squared >= radii * radii) return;
        float distance = MathF.Sqrt(squared);
        Vector3 normal = distance > 1e-7f ? delta / distance : Vector3.UnitX;
        float inverseMass = a.InverseMass + b.InverseMass;
        Vector3 correction = normal * MathF.Max(0, radii - distance - .0001f) * .85f / inverseMass;
        a.Sphere.Center -= correction * a.InverseMass;
        b.Sphere.Center += correction * b.InverseMass;
        Vector3 armA = normal * a.Sphere.Radius, armB = -normal * b.Sphere.Radius;
        Vector3 relative = b.Velocity + Vector3.Cross(b.AngularVelocity, armB)
            - a.Velocity - Vector3.Cross(a.AngularVelocity, armA);
        float speed = Vector3.Dot(relative, normal);
        if (speed >= 0) return;
        float bounce = speed < -.5f ? Restitution : 0;
        float impulse = -(1 + bounce) * speed / inverseMass;
        Vector3 force = normal * impulse;
        a.Velocity -= force * a.InverseMass; b.Velocity += force * b.InverseMass;
        Vector3 tangent = relative - normal * speed;
        float length = tangent.Length();
        if (length < 1e-7f) return;
        tangent /= length;
        float denominator = inverseMass + a.InverseInertia * Vector3.Cross(armA,tangent).LengthSquared()
            + b.InverseInertia * Vector3.Cross(armB,tangent).LengthSquared();
        Vector3 friction = -tangent * MathF.Min(length / denominator, Friction * impulse);
        a.Velocity -= friction * a.InverseMass; b.Velocity += friction * b.InverseMass;
        a.AngularVelocity -= Vector3.Cross(armA, friction) * a.InverseInertia;
        b.AngularVelocity += Vector3.Cross(armB, friction) * b.InverseInertia;
    }
}
