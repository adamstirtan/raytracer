using System;
using System.Linq;
using System.Numerics;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

using RayTracer.Core.Scenes;
using RayTracer.Core.Math;
using RayTracer.Core.Primitives;

namespace RayTracer.Core
{
    public sealed class Engine
    {
        private const int TRACE_DEPTH = 6;

        private Scene _scene;

        public Image<Rgba32> Render(int width, int height)
        {
            if (_scene == null)
            {
                return null;
            }

            Vector2 topLeft = new(-4, 3);
            Vector2 bottomRight = new(4, -3);

            float deltaX = (bottomRight.X - topLeft.X) / width;
            float deltaY = (bottomRight.Y - topLeft.Y) / height;
            float screenDeltaY = topLeft.Y;

            Image<Rgba32> render = new(width, height);

            for (int y = 0; y < height; y++)
            {
                float screenDeltaX = topLeft.X;

                Span<Rgba32> pixelRowSpan = render.GetPixelMemoryGroup().Single().Span.Slice(y * width);

                for (int x = 0; x < width; x++)
                {
                    float distance = float.MaxValue;
                    Vector3 color = Vector3.Zero;
                    Vector3 direction = new Vector3(screenDeltaX, screenDeltaY, 0) - _scene.Camera.Position;

                    Ray ray = new(_scene.Camera.Position, Vector3.Normalize(direction));
                    Raytrace(ray, ref color, 1, 1, ref distance);

                    color = Vector3.Clamp(color, Vector3.Zero, Vector3.One);

                    pixelRowSpan[x] = new Rgba32(color.X, color.Y, color.Z);

                    screenDeltaX += deltaX;
                }

                screenDeltaY += deltaY;
            }

            return render;
        }

        public void LoadScene(Scene scene)
        {
            _scene = scene;
        }

        private Primitive Raytrace(Ray ray, ref Vector3 color, int depth, float reflectionIndex, ref float distance)
        {
            if (depth > TRACE_DEPTH)
            {
                return null;
            }

            float minDistance = float.MaxValue;
            Primitive closest = null;
            RayIntersection result;

            foreach (Primitive primitive in _scene)
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

                foreach (Light light in _scene.OfType<Light>())
                {
                    Vector3 lightDirection = Vector3.Normalize(light.Center - intersection);
                    float dot = Vector3.Dot(normal, lightDirection);

                    if (dot > 0)
                    {
                        color += dot * closest.Material.Diffuse * closest.Material.Color * light.Material.Color;
                    }
                }

                if (closest.Material.Reflection > 0 && depth < TRACE_DEPTH)
                {
                    Vector3 reflectionDirection = Vector3.Reflect(ray.Direction, normal);
                    Ray reflectedRay = new(intersection + reflectionDirection * 0.001f, reflectionDirection); // Offset to avoid self-intersection
                    Vector3 reflectedColor = Vector3.Zero;
                    float reflectedDistance = float.MaxValue;

                    Raytrace(reflectedRay, ref reflectedColor, depth + 1, reflectionIndex, ref reflectedDistance);

                    color += closest.Material.Reflection * reflectedColor;
                }
            }

            return closest;
        }
    }
}