using System.Collections.Generic;
using System.Linq;

using System.Numerics;

using RayTracer.Core.Primitives;

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

        public IEnumerable<Light> Lights()
        {
            return Primitives.Where(x => x.GetPrimitiveType() == PrimitiveType.Light).AsEnumerable().Cast<Light>();
        }
    }
}