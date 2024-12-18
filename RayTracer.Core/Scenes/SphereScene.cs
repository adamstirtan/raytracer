using System;
using System.Numerics;

using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

using Plane = RayTracer.Core.Primitives.Plane;

namespace RayTracer.Core.Scenes
{
    public class SphereScene : Scene
    {
        private static readonly Vector3 cameraPosition = new(0, 0, -5);

        public SphereScene() : base(cameraPosition)
        {
            Plane ground = new(new Vector3(0f, 1f, 0f), 8f, new Material(new Vector3(0.8f, 0.8f, 0.8f),
                diffuse: 1,
                reflection: 0.8f,
                specular: 0));

            AddObject(ground);

            Random random = new(31337);

            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    Sphere sphere = new(
                        new Vector3(x * 1.5f - 3f, y * 1.5f - 3f, x * 1.5f + 3f),
                        radius: 0.5f,
                        new Material(new(random.NextSingle(), random.NextSingle(), random.NextSingle()),
                        diffuse: 0.67f,
                        reflection: 0.8f,
                        specular: 0.5f));

                    AddObject(sphere);
                }
            }

            Light light = new(cameraPosition, float.MinValue, new Material(new Vector3(1f, 1f, 1f)));

            AddLight(light);
        }
    }
}