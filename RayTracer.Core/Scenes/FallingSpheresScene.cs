using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RayTracer.Core.Physics;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

public sealed class FallingSpheresScene : ChromaticScene
{
    public SphereDynamics Dynamics { get; }
    public FallingSpheresScene() : base(falling: true)
    {
        var bodies = new List<SphereDynamics.Body>();
        int index=0;
        foreach(var sphere in this.OfType<Sphere>().Where(s=>s is not Light))
        {
            bodies.Add(new SphereDynamics.Body(sphere)
            {
                Velocity = new Vector3(System.MathF.Sin(index*1.7f)*.35f,0,System.MathF.Cos(index*.9f)*.35f)
            });
            index++;
        }
        Dynamics = new SphereDynamics(bodies);
    }
}
