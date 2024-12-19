using System.Numerics;

namespace RayTracer.Core;

public class RenderOptions
{
    public required Vector3 CameraPosition { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public int TraceDepth { get; init; } = 3;
    public required bool DisableReflections { get; init; }
    public bool DisableDiffuse { get; init; }
    public bool DisableSpeculation { get; init; }
}
