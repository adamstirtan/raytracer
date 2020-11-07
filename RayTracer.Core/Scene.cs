using System.Collections.Generic;

namespace RayTracer.Core
{
    public class Scene
    {
        private readonly List<PrimitiveBase> _primitives;

        public Scene()
        {
            _primitives = new List<PrimitiveBase>();
        }
    }
}