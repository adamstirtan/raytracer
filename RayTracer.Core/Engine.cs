using System;
using System.Numerics;

using SixLabors.ImageSharp;
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

            Vector2 topLeft = new Vector2(-4, 3);
            Vector2 bottomRight = new Vector2(4, -3);

            float deltaX = (bottomRight.X - topLeft.X) / width;
            float deltaY = (bottomRight.Y - topLeft.Y) / height;
            float screenDeltaY = topLeft.Y;

            Image<Rgba32> render = new Image<Rgba32>(width, height);

            for (int y = 0; y < height; y++)
            {
                float screenDeltaX = topLeft.X;

                Span<Rgba32> pixelRowSpan = render.GetPixelRowSpan(y);

                for (int x = 0; x < width; x++)
                {
                    float distance = 0;
                    Vector3 color = Vector3.Zero;
                    Vector3 direction = new Vector3(screenDeltaX, screenDeltaY, 0) - _scene.CameraPosition;

                    Ray ray = new Ray(_scene.CameraPosition, Vector3.Normalize(direction));
                    Raytrace(ray, ref color, 1, 1, ref distance);

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

            distance = 1000000f;
            Primitive closest = null;
            RayIntersection result;

            foreach (var primitive in _scene.Primitives)
            {
                result = primitive.Intersects(ray, ref distance);

                if (result == RayIntersection.Hit)
                {
                    closest = primitive;
                }
            }

            if (closest == null)
            {
                return null;
            }

            if (closest is Light)
            {
                color = closest.Material.Color;
            }
            else
            {
                Vector3 intersection = Vector3.Add(ray.Origin, Vector3.Multiply(distance, ray.Direction));

                foreach (var light in _scene.Lights())
                {
                    Vector3 l = Vector3.Normalize(light.Center - intersection);
                    Vector3 n = closest.GetNormal(intersection);

                    if (closest.Material.Diffuse > 0)
                    {
                        float dot = Vector3.Dot(n, l);

                        if (dot > 0)
                        {
                            color += dot * closest.Material.Diffuse * closest.Material.Color * light.Material.Color;
                        }
                    }
                }
            }

            return closest;
        }
    }
}