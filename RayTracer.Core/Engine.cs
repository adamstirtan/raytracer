using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

using RayTracer.Core.Scenes;
using RayTracer.Core.Math;
using RayTracer.Core.Primitives;

namespace RayTracer.Core;

public class Engine(Scene scene, RenderOptions options)
{
    public event EventHandler? RenderStarted;
    public event EventHandler<TimeSpan>? RenderCompleted;

    protected void OnRenderStarted()
    {
        RenderStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void OnRenderCompleted(TimeSpan time)
    {
        RenderCompleted?.Invoke(this, time);
    }

    public Image<Rgba32> Render()
    {
        OnRenderStarted();

        Stopwatch stopwatch = Stopwatch.StartNew();

        scene.Camera.Position = options.CameraPosition;

        // Define the vertical FOV (in radians)
        float verticalFOV = MathF.PI / 4; // 45 degrees

        // Calculate the aspect ratio
        float aspectRatio = (float)options.Width / options.Height;

        /// Calculate the height and width of the view plane based on the FOV and aspect ratio
        float viewPlaneHeight = 2 * MathF.Tan(verticalFOV / 2);
        float viewPlaneWidth = viewPlaneHeight * aspectRatio;

        // Calculate the top left and bottom right coordinates of the view plane
        Vector2 topLeft = new(-viewPlaneWidth / 2, viewPlaneHeight / 2);
        Vector2 bottomRight = new(viewPlaneWidth / 2, -viewPlaneHeight / 2);

        float deltaX = (bottomRight.X - topLeft.X) / options.Width;
        float deltaY = (bottomRight.Y - topLeft.Y) / options.Height;

        Image<Rgba32> render = new(options.Width, options.Height);

        Parallel.For(0, options.Height, y =>
        {
            float screenDeltaX = topLeft.X;
            float screenDeltaY = topLeft.Y + y * deltaY;

            Span<Rgba32> pixelRowSpan = render.GetPixelMemoryGroup().Single().Span[(y * options.Width)..];

            for (int x = 0; x < options.Width; x++)
            {
                float distance = float.MaxValue;
                Vector3 color = Vector3.Zero;
                Vector3 direction = new(screenDeltaX, screenDeltaY, 1);

                Ray ray = new(scene.Camera.Position, Vector3.Normalize(direction));
                Raytrace(scene, options, ray, ref color, 1, ref distance);

                color = Vector3.Clamp(color, Vector3.Zero, Vector3.One);

                pixelRowSpan[x] = new Rgba32(color.X, color.Y, color.Z);

                screenDeltaX += deltaX;
            }

            screenDeltaY += deltaY;
        });

        stopwatch.Stop();

        OnRenderCompleted(stopwatch.Elapsed);

        return render;
    }

    private static Primitive? Raytrace(Scene scene, RenderOptions options, Ray ray, ref Vector3 color, int depth, ref float distance)
    {
        if (depth > options.TraceDepth)
        {
            return null;
        }

        float minDistance = float.MaxValue;
        Primitive? closest = null;
        RayIntersection result;

        foreach (Primitive primitive in scene)
        {
            float currentDistance = distance;
            result = primitive.Intersects(ray, ref currentDistance);

            if (result == RayIntersection.Hit && currentDistance < minDistance)
            {
                minDistance = currentDistance;
                closest = primitive;
            }
        }

        if (closest == null)
        {
            color = new Vector3(0, 0, 0);
            return null;
        }

        distance = minDistance;

        if (closest is Light)
        {
            color = closest.Material.Color;
        }
        else
        {
            Vector3 intersection = Vector3.Add(ray.Origin, Vector3.Multiply(distance, ray.Direction));
            Vector3 normal = closest.GetNormal(intersection);

            Vector3 baseColor = closest.Material.Color;

            if (closest.Texture != null)
            {
                Vector2 uv = closest.GetUV(intersection);
                baseColor = closest.Texture.Sample(uv.X, uv.Y);
            }

            foreach (Light light in scene.OfType<Light>())
            {
                Vector3 lightDirection = Vector3.Normalize(light.Center - intersection);
                float dot = Vector3.Dot(normal, lightDirection);

                if (!options.DisableDiffuse && dot > 0)
                {
                    // Diffuse reflection
                    color += dot * closest.Material.Diffuse * baseColor * light.Material.Color;
                }

                // Speculation reflection
                if (!options.DisableSpeculation)
                {
                    Vector3 reflectionDirection = Vector3.Reflect(-lightDirection, normal);
                    float specularFactor = MathF.Pow(MathF.Max(Vector3.Dot(reflectionDirection, -ray.Direction), 0), closest.Material.Specular);
                    color += specularFactor * closest.Material.Specular * light.Material.Color;
                }
            }

            // Add base color contribution if diffuse is disabled
            if (options.DisableDiffuse)
            {
                color += baseColor * 0.08f; // Adjust the factor as needed
            }

            if (!options.DisableReflections && closest.Material.Reflection > 0 && depth < options.TraceDepth)
            {
                Vector3 reflectionDirection = Vector3.Reflect(ray.Direction, normal);
                Ray reflectedRay = new(intersection + reflectionDirection * 0.001f, reflectionDirection);
                Vector3 reflectedColor = Vector3.Zero;
                float reflectedDistance = float.MaxValue;

                Raytrace(scene, options, reflectedRay, ref reflectedColor, depth + 1, ref reflectedDistance);

                color += closest.Material.Reflection * reflectedColor;
            }
        }

        return closest;
    }
}