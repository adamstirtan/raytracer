using System.Collections.Generic;

namespace RayTracer.Core.Scenes
{
    public class Scene
    {
        private readonly HashSet<PrimitiveBase> _primitives;

        public Scene()
        {
            _primitives = new HashSet<PrimitiveBase>();
        }

        public int Count => _primitives.Count;

        public void AddPrimitive(PrimitiveBase primitive)
        {
            _primitives.Add(primitive);
        }
    }
}