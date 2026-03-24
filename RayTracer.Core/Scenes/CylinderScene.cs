using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

public class CylinderScene : Scene
{
    public CylinderScene()
    {
        AddObject(new RayTracer.Core.Primitives.Plane(new Vector3(0,1,0), 1000, new Material(new Vector3(0.9f,0.9f,0.9f), diffuse:0.9f), null));

        var cyl = new Cylinder(new Vector3(0,1,8), 1.0f, 2.0f, new Material(new Vector3(0.2f,0.4f,0.8f), diffuse:0.8f, reflection:0.3f), null);
        AddObject(cyl);

        AddLight(new Light(new Vector3(0, 5, -5), float.MinValue, new Material(new Vector3(0.9f,0.9f,0.9f))));
    }
}
