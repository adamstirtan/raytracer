using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

public class BoxScene : Scene
{
    public BoxScene()
    {
        AddObject(new RayTracer.Core.Primitives.Plane(new Vector3(0,1,0), 1000, new Material(new Vector3(0.9f,0.9f,0.9f), diffuse:0.9f), null));

        var box = new Box(new Vector3(-1,0,8), new Vector3(1,2,10), new Material(new Vector3(0.7f,0.2f,0.2f), diffuse:0.8f, reflection:0.2f), null);
        AddObject(box);

        AddLight(new Light(new Vector3(0, 5, -5), float.MinValue, new Material(new Vector3(0.9f,0.9f,0.9f))));
    }
}
