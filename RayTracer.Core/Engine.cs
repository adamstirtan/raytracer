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

public class Engine
{
    private readonly Scene _scene;
    private readonly RenderOptions _options;

    public event EventHandler? RenderStarted;
    public event EventHandler<TimeSpan>? RenderCompleted;

    public Engine(Scene scene, RenderOptions options)
    {
        _scene = scene ?? throw new ArgumentNullException(nameof(scene));
        _options = options ?? throw new ArgumentNullException(nameof(options));

        // Initialize camera position from options once
        _scene.Camera.Position = _options.CameraPosition;
    }

    protected void OnRenderStarted()
    {
        RenderStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void OnRenderCompleted(TimeSpan time)
    {
        RenderCompleted?.Invoke(this, time);
    }

    // Camera control API
    public void SetCameraPosition(Vector3 position) => _scene.Camera.Position = position;

    public void MoveCamera(Vector3 delta) => _scene.Camera.Position += delta;

    public void MoveCameraForward(float amount) => MoveCamera(new Vector3(0, 0, amount));
    public void MoveCameraRight(float amount) => MoveCamera(new Vector3(amount, 0, 0));
    public void MoveCameraUp(float amount) => MoveCamera(new Vector3(0, amount, 0));

    public Image<Rgba32> Render()
    {
        OnRenderStarted();

        Stopwatch stopwatch = Stopwatch.StartNew();

        // Use the current camera position (allow runtime updates)
        // _scene.Camera.Position = _options.CameraPosition;

        // Camera FOV and basis
        float verticalFOV = MathF.PI / 4; // 45 degrees
        float aspectRatio = (float)_options.Width / _options.Height;
        float viewPlaneHeight = 2 * MathF.Tan(verticalFOV / 2);
        float viewPlaneWidth = viewPlaneHeight * aspectRatio;

        // Camera basis using CameraPosition and CameraTarget
        Vector3 camPos = _scene.Camera.Position;
        Vector3 camForward = Vector3.Normalize(_options.CameraTarget - camPos);
        if (camForward == Vector3.Zero) camForward = Vector3.UnitZ;
        Vector3 worldUp = Vector3.UnitY;
        Vector3 camRight = Vector3.Normalize(Vector3.Cross(camForward, worldUp));
        Vector3 camUp = Vector3.Normalize(Vector3.Cross(camRight, camForward));

        // Top-left corner of the view plane in camera space
        Vector3 viewCenter = camPos + camForward; // distance 1
        Vector3 topLeft3D = viewCenter - (camRight * (viewPlaneWidth / 2)) + (camUp * (viewPlaneHeight / 2));

        float deltaX = viewPlaneWidth / _options.Width;
        float deltaY = viewPlaneHeight / _options.Height;

        Image<Rgba32> render = new(_options.Width, _options.Height);

        Parallel.For(0, _options.Height, y =>
        {
            Vector3 pixelStart = topLeft3D - camUp * (y * deltaY);
            Span<Rgba32> pixelRowSpan = render.GetPixelMemoryGroup().Single().Span[(y * _options.Width)..];

            for (int x = 0; x < _options.Width; x++)
            {
                float distance = float.MaxValue;
                Vector3 color = Vector3.Zero;

                Vector3 pixelPos = pixelStart + camRight * (x * deltaX);
                Vector3 direction = Vector3.Normalize(pixelPos - camPos);

                Ray ray = new(camPos, direction);
                Raytrace(_scene, _options, ray, ref color, 1, ref distance);

                color = Vector3.Clamp(color, Vector3.Zero, Vector3.One);

                pixelRowSpan[x] = new Rgba32(color.X, color.Y, color.Z);
            }
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