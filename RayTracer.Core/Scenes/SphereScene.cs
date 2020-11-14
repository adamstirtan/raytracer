using System.Numerics;

using SixLabors.ImageSharp;

using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

using Plane = RayTracer.Core.Primitives.Plane;

namespace RayTracer.Core.Scenes
{
    public class SphereScene : Scene
    {
        private static readonly Vector3 cameraPosition = new Vector3(0, 0, -5);

        public SphereScene()
            : base(cameraPosition)
        {
            Plane ground = new Plane(new Vector3(0, 0, 5f), 10, new Material(Color.Green,
                diffuse: 0,
                reflection: 1.0f,
                specular: 0));

            Sphere bigSphere = new Sphere(new Vector3(1, -0.8f, 3), 2.5f, new Material(Color.Gray,
                diffuse: 0,
                reflection: 0.6f,
                specular: 0));

            Sphere smallSphere = new Sphere(new Vector3(-5.5f, -0.5f, 7), 2f, new Material(Color.Gray,
                diffuse: 0,
                reflection: 0.6f,
                specular: 0));

            Light light1 = new Light(new Vector3(0, 5f, 5f), 0.1f, new Material(Color.Silver));
            Light light2 = new Light(new Vector3(0, 5f, 5f), 0.1f, new Material(Color.LightGray));

            Primitives.Add(ground);
            Primitives.Add(bigSphere);
            Primitives.Add(smallSphere);
            Primitives.Add(light1);
            Primitives.Add(light2);
        }
    }
}