using System.Numerics;
using System;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

// Avoid ambiguity with System.Numerics.Plane
using Plane = RayTracer.Core.Primitives.Plane;

namespace RayTracer.Core.Scenes;

public class CrystalOrchardScene : Scene
{
    public CrystalOrchardScene(int seed = 1337, int count = 450)
    {
        // Ground
        AddObject(new Plane(new Vector3(0,1,0), 1000, new Material(new Vector3(0.06f,0.06f,0.07f), diffuse:0.95f, reflection:0.15f, specular:0.2f), null));

        var rnd = new Random(seed);
        for (int i=0;i<count;i++){
            // place in a loose orchard
            float angle = (float)( rnd.NextDouble()*System.Math.PI*2.0);
            float r = (float)(2.0 + rnd.NextDouble()*25.0);
            float x = MathF.Cos(angle)*r + (float)(rnd.NextDouble()-0.5f)*0.5f;
            float z = MathF.Sin(angle)*r + (float)(rnd.NextDouble()-0.5f)*0.5f + 10f;
            float h = (float)(0.2 + rnd.NextDouble()*3.0);
            // choose prism or sphere
            if (rnd.NextDouble() < 0.25){
                // crystalline prism approximated with scaled boxes
                var size = 0.2f + (float)rnd.NextDouble()*0.8f;
                var p1 = new Vector3(x-size, 0, z-size);
                var p2 = new Vector3(x+size, h+size*2, z+size);
                var refl = (float)(0.3 + rnd.NextDouble()*0.7);
                var color = new Vector3((float)rnd.NextDouble()*0.8f+0.2f,(float)rnd.NextDouble()*0.8f+0.2f,(float)rnd.NextDouble()*0.8f+0.2f);
                AddObject(new Box(p1,p2,new Material(color,diffuse:0.1f,reflection:refl,specular:0.9f),null));
            } else {
                float radius = 0.08f + (float)rnd.NextDouble()*0.6f;
                var color = new Vector3((float)rnd.NextDouble()*0.9f+0.1f,(float)rnd.NextDouble()*0.9f+0.1f,(float)rnd.NextDouble()*0.9f+0.1f);
                var refl = (float)(0.1 + rnd.NextDouble()*0.85);
                AddObject(new Sphere(new Vector3(x, radius, z), radius, new Material(color,diffuse:(float)(0.15 + rnd.NextDouble()*0.7), reflection:refl, specular:(float)(0.1+rnd.NextDouble()*0.9)), null));
            }
        }

        // A few tall reflective columns
        for(int i=0;i<6;i++){
            float ang = i*(MathF.PI*2f/6f);
            float rx = MathF.Cos(ang)*12f;
            float rz = MathF.Sin(ang)*18f + 8f;
            AddObject(new Cylinder(new Vector3(rx,0,rz),0.6f,6f,new Material(new Vector3(0.9f,0.9f,0.95f),diffuse:0.1f,reflection:0.95f,specular:1f),null));
        }

        // Light canopy
        AddLight(new Light(new Vector3(0,40,-30), float.MinValue, new Material(new Vector3(1f,0.95f,0.9f))));
        AddLight(new Light(new Vector3(-10,20,10), float.MinValue, new Material(new Vector3(0.8f,0.9f,1f))));
        AddLight(new Light(new Vector3(12,18,15), float.MinValue, new Material(new Vector3(1f,0.8f,0.9f))));
    }
}
