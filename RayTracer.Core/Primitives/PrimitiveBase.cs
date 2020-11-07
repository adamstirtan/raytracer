using System;
using System.Collections.Generic;
using System.Text;

namespace RayTracer.Core
{
    public abstract class PrimitiveBase
    {
        public abstract IntersectionResult Intersects(Ray ray, float distance);
    }
}