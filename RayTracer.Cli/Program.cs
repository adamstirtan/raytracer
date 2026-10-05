using System.Numerics;
using RayTracer.Core;
using RayTracer.Core.Scenes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

var argsList = args;

string sceneName = "sphere";
int width = 800;
int height = 600;
string outPath = "render.png";
int depth = 2;
int samplesPerPixel = 1;
int frames = 1;
int fps = 30;

// camera defaults
Vector3? cameraPos = null;
Vector3? cameraTarget = null;

for (int i = 0; i < argsList.Length; i++)
{
    switch (argsList[i])
    {
        case "--frames":
            if (i + 1 < argsList.Length) int.TryParse(argsList[++i], out frames);
            break;
        case "--fps":
            if (i + 1 < argsList.Length) int.TryParse(argsList[++i], out fps);
            break;
        case "--scene":
            if (i + 1 < argsList.Length) sceneName = argsList[++i];
            break;
        case "--width":
            if (i + 1 < argsList.Length) int.TryParse(argsList[++i], out width);
            break;
        case "--height":
            if (i + 1 < argsList.Length) int.TryParse(argsList[++i], out height);
            break;
        case "--out":
            if (i + 1 < argsList.Length) outPath = argsList[++i];
            break;
        case "--depth":
            if (i + 1 < argsList.Length) int.TryParse(argsList[++i], out depth);
            break;
        case "--spp":
            if (i + 1 < argsList.Length) int.TryParse(argsList[++i], out samplesPerPixel);
            break;
        case "--cam-pos":
            if (i + 1 < argsList.Length)
            {
                var parts = argsList[++i].Split(',');
                if (parts.Length == 3 && float.TryParse(parts[0], out float cx) && float.TryParse(parts[1], out float cy) && float.TryParse(parts[2], out float cz))
                    cameraPos = new System.Numerics.Vector3(cx, cy, cz);
            }
            break;
        case "--cam-target":
            if (i + 1 < argsList.Length)
            {
                var parts = argsList[++i].Split(',');
                if (parts.Length == 3 && float.TryParse(parts[0], out float tx) && float.TryParse(parts[1], out float ty) && float.TryParse(parts[2], out float tz))
                    cameraTarget = new System.Numerics.Vector3(tx, ty, tz);
            }
            break;
    }
}

var scenes = new Dictionary<string, (Func<Scene> Create, Vector3 Position, Vector3? Target)>(StringComparer.OrdinalIgnoreCase)
{
    ["sphere"] = (() => new SphereScene(), new(0, 0, -5), null),
    ["triangle"] = (() => new TriangleScene(), new(0, 0, -5), null),
    ["box"] = (() => new BoxScene(), new(0, 0, -5), null),
    ["cylinder"] = (() => new CylinderScene(), new(0, 0, -5), null),
    ["disk"] = (() => new DiskScene(), new(0, 0, -5), null),
    ["billiards"] = (() => new BilliardsScene(), new(7, 8, -11), new(0, 0, 0.5f)),
    ["mesh"] = (() => new MeshScene(), new(6, 2.5f, 6), new(0, 1, 8)),
    ["hand"] = (() => new HandScene(), new(0, 0, -5), null),
    ["torus"] = (() => new TorusScene(), new(0, 0, -5), null),
    ["reflective"] = (() => new ReflectiveSphereScene(), new(0, 0, -5), null),
    ["future-city"] = (() => new FutureCityScene(), new(26, 18, -32), new(0, 5, 12)),
    ["orbital"] = (() => new OrbitalScene(), new(30, 28, -55), new(3, -1, 15)),
    ["observatory"] = (() => new ObservatoryScene(), new(17, 8, -30), new(0, 5, 12)),
    ["synthwave"] = (() => new SynthwaveScene(), new(0, 7, -12), new(0, 9, 100)),
    ["synthwave-distant"] = (() => new SynthwaveScene(distantMountains: true), new(0, 5, -12), new(0, 7, 100)),
    ["chromatic"] = (() => new ChromaticScene(), new(23,16,-28), new(0,3,12))
};

if (sceneName.Equals("list", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("Available scenes: " + string.Join(", ", scenes.Keys));
    return;
}
if (!scenes.TryGetValue(sceneName, out var preset))
{
    Console.Error.WriteLine($"Unknown scene '{sceneName}'. Use --scene list to see available scenes.");
    Environment.ExitCode = 1;
    return;
}
Scene scene = preset.Create();
cameraPos ??= preset.Position;
cameraTarget ??= preset.Target;

if (frames < 1 || fps < 1)
{
    Console.Error.WriteLine("Frames and fps must be positive.");
    Environment.ExitCode = 1;
    return;
}
if (frames > 1)
{
    if (scene is not SynthwaveScene animated)
    {
        Console.Error.WriteLine("Sequence rendering currently supports --scene synthwave.");
        Environment.ExitCode = 1;
        return;
    }
    Directory.CreateDirectory(outPath);
    for (int frame = 0; frame < frames; frame++)
    {
        float progress = (float)frame / frames;
        animated.SetAnimationProgress(progress);
        var frameOptions = new RenderOptions
        {
            Width = width, Height = height, TraceDepth = depth,
            CameraPosition = cameraPos.Value,
            CameraTarget = (cameraTarget ?? new Vector3(0, 9, 100)),
            DisableReflections = false, SamplesPerPixel = samplesPerPixel
        };
        using var frameImage = new Engine(scene, frameOptions).Render();
        frameImage.SaveAsPng(Path.Combine(outPath, $"frame-{frame:00000}.png"));
        if (frame % fps == 0 || frame == frames - 1)
            Console.WriteLine($"Rendered {frame + 1}/{frames} frames");
    }
    Console.WriteLine($"Saved sequence to {outPath}");
    return;
}

var options = new RenderOptions
{
    Width = width,
    Height = height,
    TraceDepth = depth,
    CameraPosition = cameraPos.Value,
    CameraTarget = cameraTarget,
    DisableReflections = false,
    SamplesPerPixel = samplesPerPixel
};

var engine = new Engine(scene, options);
engine.RenderStarted += (_, __) => Console.WriteLine("Render started");
engine.RenderCompleted += (_, ts) => Console.WriteLine($"Render completed in {ts}");

using Image<Rgba32> image = engine.Render();

image.SaveAsPng(outPath);
Console.WriteLine($"Saved to {outPath}");
