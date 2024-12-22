using System.Numerics;

using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

public class TexturedSphere : Scene
{
    public TexturedSphere()
    {
        Texture earthTexture = new("Textures/earth.jpg");
        Texture moonTexture = new("Textures/moon.jpg");

        Sphere earth = new(new Vector3(-1, 0, 15f), 4.5f, new Material(new Vector3(0, 0, 0),
            diffuse: 1f,
            reflection: 0.2f,
            specular: 0.6f),
            earthTexture);

        Sphere moon = new(new Vector3(6, 2, 22f), 2.2f, new Material(new Vector3(0, 0, 0),
            diffuse: 1f,
            reflection: 0.4f,
            specular: 0.1f),
            moonTexture);

        AddObject(earth);
        AddObject(moon);

        AddLight(new(new Vector3(0, 50, -50), float.MinValue, new Material(new Vector3(1, 1, 1))));
    }
}
