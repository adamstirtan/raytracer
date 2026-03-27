using System.Numerics;

namespace RayTracer.Core;

public class RenderOptions
{
    public required Vector3 CameraPosition { get; init; }
    // New: point the camera looks at (defaults to origin if not set)
    public Vector3 CameraTarget { get; init; } = Vector3.Zero;
    public required int Width { get; init; }
    public required int Height { get; init; }
    public int TraceDepth { get; init; } = 3;
    public required bool DisableReflections { get; init; }
    public bool DisableDiffuse { get; init; }
    public bool DisableSpeculation { get; init; }

    // Supersampling: number of samples per pixel (1 = no supersampling).
    // Use perfect squares like 1,4,9 (1x1,2x2,3x3 stratified sampling).
    public int SamplesPerPixel { get; init; } = 1;
}
