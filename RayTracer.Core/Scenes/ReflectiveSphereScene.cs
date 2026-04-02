using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

public class ReflectiveSphereScene : Scene
{
    public ReflectiveSphereScene()
    {
        // Ground plane
        AddObject(new Primitives.Plane(new Vector3(0, 1, 0), 1000, new Material(new Vector3(0.12f, 0.12f, 0.12f), diffuse: 0.9f, reflection: 0.15f, specular: 0.2f), null));

        // Arrange ~10 spheres in a loose cluster to showcase reflections
        var center = new Vector3(0f, 0.6f, 8f);
        var positions = new Vector3[] {
            new Vector3( -3.0f, 0.45f, 7.5f),
            new Vector3( -1.6f, 0.6f, 8.8f),
            new Vector3( -0.2f, 0.5f, 7.3f),
            new Vector3( 1.2f, 0.55f, 8.1f),
            new Vector3( 2.6f, 0.4f, 7.8f),
            new Vector3( 0.0f, 1.2f, 9.2f),
            new Vector3( -2.4f, 1.0f, 9.6f),
            new Vector3( 2.0f, 1.1f, 9.9f),
            new Vector3( -0.8f, 0.9f, 10.6f),
            new Vector3( 1.6f, 0.75f, 11.0f)
        };

        float[] radii = new float[] { 0.45f, 0.6f, 0.4f, 0.5f, 0.35f, 0.85f, 0.7f, 0.6f, 0.55f, 0.65f };

        // Create reflective materials with slight variation in tint
        for (int i = 0; i < positions.Length; i++)
        {
            var pos = positions[i];
            var radius = radii[i % radii.Length];
            var tint = new Vector3(
                0.3f + 0.6f * (i % 3 == 0 ? 1f : 0.6f),
                0.25f + 0.5f * ((i + 1) % 3 == 0 ? 1f : 0.45f),
                0.2f + 0.7f * ((i + 2) % 3 == 0 ? 1f : 0.5f)
            );

            var mat = new Material(tint, diffuse: 0.08f, reflection: 0.92f, specular: 1f);
            AddObject(new Sphere(pos, radius, mat, null));
        }

        // A few semi-matte spheres to add contrast
        AddObject(new Sphere(new Vector3(-4.2f, 0.5f, 9.0f), 0.6f, new Material(new Vector3(0.15f,0.18f,0.25f), diffuse: 0.7f, reflection: 0.05f, specular: 0.15f), null));
        AddObject(new Sphere(new Vector3(4.0f, 0.6f, 8.6f), 0.7f, new Material(new Vector3(0.9f,0.85f,0.7f), diffuse: 0.6f, reflection: 0.1f, specular: 0.2f), null));

        // Lights: soft key and fill lights to produce visible reflections/highlights
        AddLight(new Light(new Vector3(4f, 8f, -6f), float.MinValue, new Material(new Vector3(1f, 0.98f, 0.95f))));
        AddLight(new Light(new Vector3(-6f, 6f, -4f), float.MinValue, new Material(new Vector3(0.8f, 0.9f, 1f))));
        AddLight(new Light(new Vector3(0f, 10f, 6f), float.MinValue, new Material(new Vector3(0.6f, 0.6f, 0.7f))));

        // Camera target
        Camera.Target = center;
    }
}
