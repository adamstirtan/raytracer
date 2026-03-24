using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

public class MeshScene : Scene
{
    public MeshScene()
    {
        AddObject(new RayTracer.Core.Primitives.Plane(new Vector3(0,1,0), 1000, new Material(new Vector3(0.2f,0.2f,0.2f), 0.9f, 0f, 0.1f), null));

        var mesh = Mesh.FromObj("assets/teapot.obj", new Material(new Vector3(0.8f,0.5f,0.3f), 0.7f, 0.2f, 0.5f));
        AddObject(mesh);

        AddLight(new Light(new Vector3(0, 8, -5), float.MinValue, new Material(new Vector3(1f,1f,0.95f))));
    }
}
