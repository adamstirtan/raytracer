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
    "list" => throw new System.ArgumentException("list is not a scene"),
    _ => new SphereScene()
};

if (sceneName.ToLower() == "list")
{
    Console.WriteLine("Available scenes: sphere, triangle, box, cylinder, disk");
    System.Environment.Exit(0);
}

var options = new RenderOptions
{
    Width = width,
    Height = height,
    TraceDepth = depth,
    CameraPosition = new Vector3(0, 0, -5),
    DisableReflections = false
};

var engine = new Engine(scene, options);
engine.RenderStarted += (_, __) => Console.WriteLine("Render started");
engine.RenderCompleted += (_, ts) => Console.WriteLine($"Render completed in {ts}");

using Image<Rgba32> image = engine.Render();

image.SaveAsPng(outPath);
Console.WriteLine($"Saved to {outPath}");
