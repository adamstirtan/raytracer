using System;
using System.IO;
using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

public class BilliardsScene : Scene
{
    public BilliardsScene()
    {
        string directory = Path.Combine(AppContext.BaseDirectory, "Textures", "Billiards");
        var felt = new Texture(Path.Combine(directory, "felt.png"));
        var walnut = new Texture(Path.Combine(directory, "walnut.png"));
        var clothMaterial = new Material(Vector3.One, 0.85f, 0, 0);
        var woodMaterial = new Material(Vector3.One, 0.75f, 0.08f, 0.08f) { Shininess = 40 };
        var cushion = new Material(new Vector3(0.035f, 0.22f, 0.14f), 0.8f, 0, 0);
        var black = new Material(new Vector3(0.012f), 0.5f, 0, 0);
        var brass = new Material(new Vector3(0.62f, 0.4f, 0.12f), 0.7f, 0.2f, 0.15f) { Shininess = 48 };

        void Block(Vector3 min, Vector3 max, Material material, Texture? texture = null)
            => AddObject(new Box(min, max, material, texture));

        // A finite cloth bed: balls and pocket mouths share the same surface height.
        AddObject(new FeltBed(new Vector3(-3, -0.2f, -6), new Vector3(3, 0, 6), clothMaterial, felt));
        Block(new(-3.65f,-0.8f,-6.65f),new(3.65f,-0.24f,6.65f),woodMaterial,walnut);
        foreach (float x in new[] { -3.35f, 3.35f })
        {
            Block(new(x-.3f,-.18f,-6.6f),new(x+.3f,.25f,6.6f),woodMaterial,walnut);
            foreach (float z in new[] {-3f,3f})
                Block(new(x>0?2.85f:-3.08f,0,z-2.55f),new(x>0?3.08f:-2.85f,.18f,z+2.55f),cushion);
            for(int i=-5;i<=5;i+=2)
                AddObject(new Sphere(new(x,.26f,i),.065f,new Material(new Vector3(.85f),.8f,0,0),null));
        }
        foreach(float z in new[]{-6.35f,6.35f})
        {
            Block(new(-3.65f,-.18f,z-.3f),new(3.65f,.25f,z+.3f),woodMaterial,walnut);
            Block(new(-2.6f,0,z>0?5.85f:-6.08f),new(2.6f,.18f,z>0?6.08f:-5.85f),cushion);
        }
        // Dark inset pocket mouths and brass collars.
        foreach(float x in new[]{-2.95f,2.95f})
            foreach(float z in new[]{-5.95f,0f,5.95f})
            {
                AddObject(new Disk(new(x,.005f,z),Vector3.UnitY,.34f,black,null));
                AddObject(new Torus(brass,null,.35f,.045f,new(x,.025f,z)));
            }
        foreach(float x in new[]{-2.7f,2.7f})
            foreach(float z in new[]{-5.2f,5.2f})
                Block(new(x-.22f,-2.6f,z-.22f),new(x+.22f,-.7f,z+.22f),woodMaterial,walnut);
        AddObject(new Primitives.Plane(Vector3.UnitY,2.65f,
            new Material(new Vector3(.045f,.04f,.035f),.75f,0,0),null));

        const float radius = .24f;
        const float spacing = radius * 2.025f;
        int[] numbers = {1,9,2,10,8,3,11,4,12,5,6,13,7,14,15};
        int index=0;
        for(int row=0;row<5;row++)
            for(int column=0;column<=row;column++)
            {
                int number=numbers[index++];
                var texture=new Texture(Path.Combine(directory,$"ball-{number:00}.png"));
                AddObject(new Sphere(new((column-row*.5f)*spacing,radius,1.2f+row*spacing*.8660254f),radius,
                    new Material(Vector3.One,.78f,.12f,.32f){Shininess=80},texture));
            }
        AddObject(new Sphere(new(.3f,radius,-2.1f),radius,
            new Material(new Vector3(.94f,.93f,.88f),.8f,.12f,.32f){Shininess=80},null));

        AddLight(new Light(new(-3,7,-1),float.MinValue,new Material(new Vector3(1.15f,1.12f,1.04f))));
        AddLight(new Light(new(4,6,3),float.MinValue,new Material(new Vector3(.5f,.55f,.6f))));
        Camera.Target=new Vector3(0,0,1.8f);
    }

    private sealed class FeltBed(Vector3 min, Vector3 max, Material material, Texture texture)
        : Box(min,max,material,texture)
    {
        public override Vector2 GetUV(Vector3 position) => new(position.X*1.4f,position.Z*1.4f);
    }
}
