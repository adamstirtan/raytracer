using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

public class Scene : IEnumerable<Primitive>, IEnumerable<Light>
{
    private readonly ICollection<Primitive> _objects = [];
    private readonly ICollection<Light> _lights = [];

    public Scene()
    {
        Camera = new Camera
        {
            Position = Vector3.Zero
        };
    }

    public Camera Camera { get; init; }

    public void AddObject(Primitive primitive)
    {
        _objects.Add(primitive);
    }

    public void AddLight(Light light)
    {
        AddObject(light);

        _lights.Add(light);
    }

    public IEnumerator<Primitive> GetEnumerator()
    {
        return _objects.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _objects.GetEnumerator();
    }

    IEnumerator<Light> IEnumerable<Light>.GetEnumerator()
    {
        return _lights.GetEnumerator();
    }
}