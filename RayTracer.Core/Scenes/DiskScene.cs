using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

public class DiskScene : Scene
{
    public DiskScene()
    {
        AddObject(new RayTracer.Core.Primitives.Plane(new Vector3(0,1,0), 1000, new Material(new Vector3(0.9f,0.9f,0.9f), diffuse:0.9f), null));

        var disk = new Disk(new Vector3(0,0,6), new Vector3(0,1,0), 2.0f, new Material(new Vector3(0.8f,0.8f,0.2f), diffuse:0.8f, reflection:0.1f), null);
        AddObject(disk);

        AddLight(new Light(new Vector3(0, 5, -5), float.MinValue, new Material(new Vector3(0.9f,0.9f,0.9f))));
    }
}
