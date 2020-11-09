using System.Collections.Generic;

namespace RayTracer.Core.Scenes
{
    public class Scene
    {
        private readonly HashSet<Primitive> _primitives;

        public Scene()
        {
            _primitives = new HashSet<Primitive>();
        }

        public int Count => _primitives.Count;

        public void AddPrimitive(Primitive primitive)
        {
            _primitives.Add(primitive);
        }
    }
}