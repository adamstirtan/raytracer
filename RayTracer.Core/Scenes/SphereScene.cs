using System.Numerics;

using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes
{
    public class SphereScene : Scene
    {
        public SphereScene()
        {
            Sphere sphere = new Sphere(new Vector3(0, 0, 5f), 10);

            AddPrimitive(sphere);
        }
    }
}