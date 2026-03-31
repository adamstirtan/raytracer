using System.Numerics;

namespace RayTracer.Core.Scenes;
public class Camera
{
    public required Vector3 Position { get; set; } = Vector3.Zero;

    // Optional target/look-at point. When set, Engine will use this target to orient the camera.
    public Vector3? Target { get; set; }

    private Vector3? Rotation { get; set; } = Vector3.Zero;
    private Vector3? Velocity { get; set; } = Vector3.Zero;
}
