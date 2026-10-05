using System;
using System.Collections.Generic;
using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

/// <summary>A polished sphere garden and suspended double helix in a reflective studio.</summary>
public class ChromaticScene : Scene
{
    public ChromaticScene()
    {
        var spheres = new List<Sphere>();
        Vector3[] palette = { new(.05f,.55f,.65f), new(.8f,.2f,.065f), new(.55f,.09f,.38f),
            new(.16f,.22f,.7f), new(.85f,.6f,.18f), new(.7f,.78f,.85f) };
        bool Place(Vector3 position, float radius, int color)
        {
            foreach (var sphere in spheres)
                if (Vector3.Distance(position, sphere.Center) < radius + sphere.Radius + .12f) return false;
            var material = new Material(palette[color % palette.Length], .48f, .48f, .2f) { Shininess = 96 };
            if (color % 6 == 5) { material.Diffuse = .12f; material.Reflection = .82f; }
            var item = new Sphere(position, radius, material, null);
            spheres.Add(item); AddObject(item);
            return true;
        }

        Place(new Vector3(0,2.2f,11),2.2f,5);
        Place(new Vector3(-5,1.5f,5),1.5f,0);
        Place(new Vector3(5,1.7f,7),1.7f,4);
        // Two rigid, interwoven spirals with ample separation between neighboring spheres.
        for (int strand=0;strand<2;strand++)
            for (int i=0;i<36;i++)
            {
                float angle = i * .34f + strand * MathF.PI;
                float radius = 5.2f + i * .035f;
                Place(new Vector3(MathF.Cos(angle)*radius,2.6f+i*.23f,11+MathF.Sin(angle)*radius),.43f,i+strand*3);
            }
        var random = new Random(2026);
        for(int attempt=0;spheres.Count<360 && attempt<100000;attempt++)
        {
            float radius = .25f+(float)random.NextDouble()*.62f;
            float x = ((float)random.NextDouble()-.5f)*34;
            float z = -5+(float)random.NextDouble()*38;
            Place(new Vector3(x,radius,z),radius,attempt);
        }
        if(spheres.Count!=360) throw new InvalidOperationException("Could not place the sphere garden.");

        AddObject(new Primitives.Plane(Vector3.UnitY,0,
            new Material(new Vector3(.055f,.07f,.1f),.55f,.55f,.1f){Shininess=100},null));
        // Large luminous panels appear in the polished surfaces as broad studio reflections.
        void Panel(Vector3 min,Vector3 max,Vector3 color)
            => AddObject(new Box(min,max,new Material(color){Emission=1},null));
        Panel(new(-70,12,-80),new(-69,20,-50),new(.18f,.7f,1f));
        Panel(new(69,12,-80),new(70,20,-50),new(1f,.4f,.16f));
        Panel(new(-13,32,2),new(13,32.1f,5),new(.9f,.94f,1f));
        Panel(new(-13,32,20),new(13,32.1f,23),new(.8f,.85f,1f));
        AddLight(new Light(new(-12,18,-10),float.MinValue,new Material(new Vector3(.8f,1f,1.2f))));
        AddLight(new Light(new(14,14,8),float.MinValue,new Material(new Vector3(1.2f,.8f,.55f))));
        AddLight(new Light(new(0,22,24),float.MinValue,new Material(new Vector3(.8f,.85f,1f))));
        Camera.Target = new Vector3(0,3,12);
    }
}
