using System.Numerics;

using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

using Plane = RayTracer.Core.Primitives.Plane;

namespace RayTracer.Core.Scenes;

public class SphereScene : Scene
{
    public SphereScene()
    {
        Plane ground = new(new Vector3(0, 1, 0), 1000, new Material(new Vector3(0.8f, 0.8f, 0.8f),
            diffuse: 0.93f,
            reflection: 0.8f,
            specular: 0.6f));

        AddObject(ground);

        Sphere sphere1 = new(new Vector3(-1, 0, 5f), 1f, new Material(new Vector3(1, 0, 0),
            diffuse: 1f,
            reflection: 0.7f,
            specular: 0.5f));

        Sphere sphere2 = new(new Vector3(1.8f, -1.4f, 3f), 1.6f, new Material(new Vector3(0, 1, 0),
            diffuse: 0.8f,
            reflection: 0.9f,
            specular: 0.5f));

        Sphere sphere3 = new(new Vector3(1, 2.1f, 2.4f), 1.5f, new Material(new Vector3(0, 0, 1),
            diffuse: 0.8f,
            reflection: 0.2f,
            specular: 0.1f));

        AddObject(sphere1);
        AddObject(sphere2);
        AddObject(sphere3);

        Triangle triangle = new(new Vector3(-1, 0, 7), new Vector3(5, 0, 7), new Vector3(0, 1, 7), new Material(new Vector3(0.2f, 0.53f, 0.6f),
            diffuse: 0.8f,
            reflection: 0.2f,
            specular: 0.5f));

        AddObject(triangle);

        Light light = new(new Vector3(-7, 0, -9), float.MinValue, new Material(new Vector3(1f, 1f, 1f)));

        AddLight(light);
    }
}