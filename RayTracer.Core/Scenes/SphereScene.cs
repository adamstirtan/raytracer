using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;
using System.Numerics;

namespace RayTracer.Core.Scenes;

public class SphereScene : Scene
{
    public SphereScene()
    {
        // Ground plane at y = 0 — slightly reflective to pick up the sphere's colour
        Primitives.Plane ground = new(new Vector3(0, 1, 0), 0, new Material(new Vector3(0.15f, 0.15f, 0.15f),
            diffuse: 0.85f,
            reflection: 0.25f,
            specular: 0.15f),
            null);

        AddObject(ground);

        // Main sphere resting on the ground (center.Y == radius)
        Sphere mainSphere = new(new Vector3(0, 1.5f, 0), 1.5f, new Material(new Vector3(0.9f, 0.08f, 0.05f),
            diffuse: 0.8f,
            reflection: 0.55f,
            specular: 0.9f),
            null);

        AddObject(mainSphere);

        // Smaller accent sphere for depth and visual interest — also resting on the ground
        Sphere accentSphere = new(new Vector3(-2.8f, 0.6f, 1.5f), 0.6f, new Material(new Vector3(0.1f, 0.35f, 0.9f),
            diffuse: 0.75f,
            reflection: 0.6f,
            specular: 0.85f),
            null);

        AddObject(accentSphere);

        // Key light: warm white, high and to the right
        AddLight(new Light(new Vector3(5f, 8f, -3f), float.MinValue, new Material(new Vector3(1f, 0.95f, 0.88f))));

        // Fill light: cool blue, opposite side — softens shadows without washing out colour
        AddLight(new Light(new Vector3(-4f, 5f, -5f), float.MinValue, new Material(new Vector3(0.45f, 0.55f, 0.8f))));

        // Aim camera at the base of the main sphere so some ground is visible
        Camera.Target = new Vector3(0, 1.2f, 0);
    }
}