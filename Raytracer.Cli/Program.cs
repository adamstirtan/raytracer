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

for (int i = 0; i < argsList.Length; i++)
{
    switch (argsList[i])
    {
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

Scene scene = sceneName.ToLower() switch
{
    "sphere" => new SphereScene(),
    "triangle" => new TriangleScene(),
    "box" => new BoxScene(),
    "cylinder" => new CylinderScene(),
    "disk" => new DiskScene(),
    "billiards" => new BilliardsScene(),
    "mesh" => new MeshScene(),
    "hand" => new HandScene(),
    "torus" => new RayTracer.Core.Scenes.TorusScene(),
    "reflective" => new RayTracer.Core.Scenes.ReflectiveSphereScene(),
    "list" => throw new System.ArgumentException("list is not a scene"),
    _ => new SphereScene()
};

if (sceneName.ToLower() == "list")
{
    Console.WriteLine("Available scenes: sphere, triangle, box, cylinder, disk, billiards, mesh");
    System.Environment.Exit(0);
}

// Choose camera position; default behind the view looking forward
Vector3 cameraPos = new Vector3(0, 0, -5);
Vector3 cameraTarget = Vector3.Zero;
if (sceneName.ToLower() == "billiards")
{
    // Overhead camera above table, looking downwards
    cameraPos = new Vector3(0, 8, 0);
    cameraTarget = new Vector3(0, 0, 0);
}
else if (sceneName.ToLower() == "mesh")
{
    // Position camera to the front-left-top of the model and look at its center
    cameraPos = new Vector3(6f, 2.5f, 6f);
    cameraTarget = new Vector3(0f, 1.0f, 8.0f);
}

var options = new RenderOptions
{
    Width = width,
    Height = height,
    TraceDepth = depth,
    CameraPosition = cameraPos,
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
