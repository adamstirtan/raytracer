using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

public class TriangleScene : Scene
{
    public TriangleScene()
    {
        // Ground plane
        AddObject(new Plane(new Vector3(0,1,0), 1000, new Material(new Vector3(0.8f,0.8f,0.8f), diffuse:0.9f, reflection:0.2f, specular:0.1f), null));

        // Single triangle in front of camera
        var a = new System.Numerics.Vector3(-1, 0, 5);
        var b = new System.Numerics.Vector3(1, 0, 5);
        var c = new System.Numerics.Vector3(0, 2, 5);

        AddObject(new Triangle(a, b, c, new Material(new Vector3(0.2f,0.7f,0.3f), diffuse:0.8f, reflection:0.3f, specular:0.5f), null));

        // Add a light
        AddLight(new Light(new Vector3(0, 5, -5), float.MinValue, new Material(new Vector3(0.9f,0.9f,0.9f))));
    }
}
