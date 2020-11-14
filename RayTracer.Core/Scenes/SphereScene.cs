using System.Numerics;

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
            Plane ground = new Plane(new Vector3(0, 1, 0), 4.4f, new Material(new Vector3(0.4f, 0.3f, 0.3f),
                diffuse: 1,
                reflection: 0,
                specular: 0));

            Sphere bigSphere = new Sphere(new Vector3(0, 0, 10), 2.5f, new Material(new Vector3(0.7f, 0.7f, 0.7f),
                diffuse: 0,
                reflection: 0.6f,
                specular: 0));

            Sphere smallSphere = new Sphere(new Vector3(-5.5f, -0.5f, 7), 2f, new Material(new Vector3(0.7f, 0.7f, 0.7f),
                diffuse: 0.1f,
                reflection: 0,
                specular: 0));

            Light light1 = new Light(new Vector3(0, 5, 5), 0.1f, new Material(new Vector3(0.6f, 0.6f, 0.6f)));
            Light light2 = new Light(new Vector3(2, 5, 1), 0.1f, new Material(new Vector3(0.7f, 0.6f, 0.9f)));

            Primitives.Add(ground);
            Primitives.Add(bigSphere);
            Primitives.Add(smallSphere);
            Primitives.Add(light1);
            Primitives.Add(light2);
        }
    }
}