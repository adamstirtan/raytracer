using System.Numerics;

namespace RayTracer.Core
{
    public class Ray
    {
        private Vector3 _origin;
        private Vector3 _destination;

        public Ray()
            : this(Vector3.Zero, Vector3.Zero)
        { }

        public Ray(Vector3 origin, Vector3 destination)
        {
            _origin = origin;
            _destination = destination;
        }
    }
}