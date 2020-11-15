using System.Numerics;

namespace RayTracer.Core.Math
{
    public class Ray
    {
        public Vector3 Origin { get; set; }
        public Vector3 Direction { get; set; }

        public Ray()
            : this(Vector3.Zero, Vector3.Zero)
        { }

        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }
    }
}