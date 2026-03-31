using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

public class BilliardsScene : Scene
{
    public BilliardsScene()
    {
        // Table plane
        AddObject(new RayTracer.Core.Primitives.Plane(new Vector3(0,1,0), 1000,
            new Material(new Vector3(0.05f,0.4f,0.05f), 0.9f, 0.1f, 0.2f), null));

        // Walls (short raised borders) as boxes
        AddObject(new Box(new Vector3(-5,0.2f,3), new Vector3(5,0.6f,3.2f), new Material(new Vector3(0.1f,0.05f,0.02f), 0.2f, 0.3f, 0.1f), null));
        AddObject(new Box(new Vector3(-5,0.2f,-3.2f), new Vector3(5,0.6f,-3), new Material(new Vector3(0.1f,0.05f,0.02f), 0.2f, 0.3f, 0.1f), null));
        AddObject(new Box(new Vector3(-5,0.2f,-3), new Vector3(-4.8f,0.6f,3), new Material(new Vector3(0.1f,0.05f,0.02f), 0.2f, 0.3f, 0.1f), null));
        AddObject(new Box(new Vector3(4.8f,0.2f,-3), new Vector3(5,0.6f,3), new Material(new Vector3(0.1f,0.05f,0.02f), 0.2f, 0.3f, 0.1f), null));

        // Add a light above
        AddLight(new Light(new Vector3(0, 8, -5), float.MinValue, new Material(new Vector3(1f,1f,0.95f))));

        // Add balls (approximate positions). Cue ball white at front
        var balls = new (Vector3 pos, Vector3 color)[]
        {
            (new Vector3(0,0.2f, -1.5f), new Vector3(1f,1f,1f)), // cue
            (new Vector3(0,0.2f, 1.5f), new Vector3(1f,1f,0f)), // 1 yellow
            (new Vector3(-0.6f,0.2f,1.8f), new Vector3(0f,0f,1f)), // 2 blue
            (new Vector3(0.6f,0.2f,1.8f), new Vector3(1f,0f,0f)), // 3 red
            (new Vector3(-1.2f,0.2f,2.1f), new Vector3(0.5f,0f,0.5f)), // 4 purple
            (new Vector3(0,0.2f,2.1f), new Vector3(1f,0.5f,0f)), // 5 orange
            (new Vector3(1.2f,0.2f,2.1f), new Vector3(0f,0.5f,0f)), // 6 green
            (new Vector3(-1.8f,0.2f,2.4f), new Vector3(0.6f,0.6f,0.6f)), // 7 maroon-ish
            (new Vector3(-0.6f,0.2f,2.4f), new Vector3(0.2f,0.2f,0.6f)), // 8 dark
            (new Vector3(0.6f,0.2f,2.4f), new Vector3(0.9f,0.9f,0.2f)),
            (new Vector3(1.8f,0.2f,2.4f), new Vector3(0.2f,0.9f,0.9f))
        };

        foreach (var (pos, color) in balls)
        {
            AddObject(new Sphere(pos, 0.2f, new Material(color, 0.7f, 0.5f, 0.6f), null));
        }
    }
}
