using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

public class TorusScene : Scene
{
    public TorusScene()
    {
        Primitives.Plane ground = new(new Vector3(0, 1, 0), 0, new Material(new Vector3(0.15f, 0.15f, 0.15f),
            diffuse: 0.85f,
            reflection: 0.75f,
            specular: 0.8f),
            null);

        AddObject(ground);

        var torus = new Torus(new Material(new Vector3(0.8f,0.3f,0.2f), 0.6f, 0.2f, 0.3f), null, major:2.0f, minor:0.5f, center: new Vector3(0, 0.5f, 0));
        AddObject(torus);

        // Key light: warm white, high and to the right
        AddLight(new Light(new Vector3(5f, 8f, -3f), float.MinValue, new Material(new Vector3(1f, 0.95f, 0.88f))));

        // Fill light: cool blue, opposite side — softens shadows without washing out colour
        AddLight(new Light(new Vector3(-4f, 5f, -5f), float.MinValue, new Material(new Vector3(0.45f, 0.55f, 0.8f))));

        // Aim camera at the base of the main sphere so some ground is visible
        Camera.Target = new Vector3(0, 0.5f, 0);
    }
}
