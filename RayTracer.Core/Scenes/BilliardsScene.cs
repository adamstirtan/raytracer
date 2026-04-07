using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

public class BilliardsScene : Scene
{
    public BilliardsScene()
    {
        // Table plane
        AddObject(new RayTracer.Core.Primitives.Plane(new Vector3(0,1,0), 1000,
            new Material(new Vector3(0.05f,0.4f,0.05f), 0.9f, 0.1f, 0.2f), null));

        // Walls (short raised borders) as boxes
        AddObject(new Box(new Vector3(-5,0.2f,3), new Vector3(5,0.6f,3.2f), new Material(new Vector3(0.1f,0.05f,0.02f), 0.2f, 0.3f, 0.1f), null));
        AddObject(new Box(new Vector3(-5,0.2f,-3.2f), new Vector3(5,0.6f,-3), new Material(new Vector3(0.1f,0.05f,0.02f), 0.2f, 0.3f, 0.1f), null));
        AddObject(new Box(new Vector3(-5,0.2f,-3), new Vector3(-4.8f,0.6f,3), new Material(new Vector3(0.1f,0.05f,0.02f), 0.2f, 0.3f, 0.1f), null));
        AddObject(new Box(new Vector3(4.8f,0.2f,-3), new Vector3(5,0.6f,3), new Material(new Vector3(0.1f,0.05f,0.02f), 0.2f, 0.3f, 0.1f), null));

        // Add a light above
        AddLight(new Light(new Vector3(0, 8, -5), float.MinValue, new Material(new Vector3(1f,1f,0.95f))));

        // Build a denser triangular rack of balls (15 balls) using hexagonal close packing
        float radius = 0.2f;
        float spacing = radius * 2.0f * 0.98f; // slightly tighter than diameter
        float rowZSpacing = spacing * 0.8660254f; // sqrt(3)/2 for hex packing
        Vector3 rackApex = new Vector3(0f, 0.2f, 1.8f);

        var colors = new Vector3[]
        {
            new Vector3(1f,1f,0f), // yellow
            new Vector3(0f,0f,1f), // blue
            new Vector3(1f,0f,0f), // red
            new Vector3(0.5f,0f,0.5f), // purple
            new Vector3(1f,0.5f,0f), // orange
            new Vector3(0f,0.5f,0f), // green
            new Vector3(0.6f,0.6f,0.6f), // gray
            new Vector3(0.2f,0.2f,0.6f), // dark blue
            new Vector3(0.9f,0.9f,0.2f), // light yellow
            new Vector3(0.2f,0.9f,0.9f), // cyan
            new Vector3(1f,0.8f,0.6f), // beige
            new Vector3(0.8f,0.1f,0.6f), // magenta
            new Vector3(0.3f,0.3f,0.3f), // dark
            new Vector3(0.1f,0.6f,0.2f),
            new Vector3(0.9f,0.3f,0.1f)
        };

        int colorIdx = 0;
        int rows = 5; // 1+2+3+4+5 = 15 balls
        for (int r = 0; r < rows; r++)
        {
            int count = r + 1;
            float startX = -((count - 1) * spacing) / 2.0f;
            float z = rackApex.Z + r * rowZSpacing;
            for (int i = 0; i < count; i++)
            {
                float x = startX + i * spacing;
                var pos = new Vector3(x, rackApex.Y, z);
                var color = colors[colorIdx % colors.Length];
                AddObject(new Sphere(pos, radius, new Material(color, 0.7f, 0.5f, 0.6f), null));
                colorIdx++;
            }
        }

        // Cue ball placed lower on the table in front of the rack
        AddObject(new Sphere(new Vector3(0f, 0.2f, -2.0f), radius, new Material(new Vector3(1f,1f,1f), 0.7f, 0.5f, 0.6f), null));
    }
}
