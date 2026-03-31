using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

public class TorusScene : Scene
{
    public TorusScene()
    {
        AddObject(new RayTracer.Core.Primitives.Plane(new Vector3(0,1,0), 1000, new Material(new Vector3(0.2f,0.2f,0.2f), 0.9f, 0f, 0.1f), null));

        var torus = new Torus(new Material(new Vector3(0.8f,0.3f,0.2f), 0.6f, 0.2f, 0.3f), null, major:2.0f, minor:0.5f);
        AddObject(torus);

        AddLight(new Light(new Vector3(4, 6, -5), float.MinValue, new Material(new Vector3(1f,1f,0.95f))));
    }
}
