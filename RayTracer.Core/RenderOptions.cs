using System.Numerics;

namespace RayTracer.Core;

public class RenderOptions
{
    public required Vector3 CameraPosition { get; init; }
    // Unset uses the scene's target, then the origin. Zero explicitly targets the origin.
    public Vector3? CameraTarget { get; init; }
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
