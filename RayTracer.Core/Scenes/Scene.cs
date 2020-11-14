using System.Collections.Generic;
using System.Numerics;

namespace RayTracer.Core.Scenes
{
    public class Scene
    {
        public List<Primitive> Primitives;
        public Vector3 CameraPosition;

        public Scene(Vector3 cameraPosition)
        {
            Primitives = new List<Primitive>();
            CameraPosition = cameraPosition;
        }
    }
}