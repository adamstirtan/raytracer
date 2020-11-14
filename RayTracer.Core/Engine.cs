using System;
using System.Numerics;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

using RayTracer.Core.Scenes;
using RayTracer.Core.Primitives;

namespace RayTracer.Core
{
    public sealed class Engine
    {
        private const int TRACE_DEPTH = 6;

        private Scene _scene;
        private float _wx1;
        private float _wy1;
        private float _wx2;
        private float _wy2;
        private float _dx;
        private float _dy;
        private float _sx;
        private float _sy;

        public string Render(int width, int height)
        {
            if (_scene == null)
            {
                return null;
            }

            // Screen plane in world space coordinates
            _wx1 = -4;
            _wx2 = 4;
            _wy1 = _sy = 3;
            _wy2 = -3;

            // Deltas for interpolation
            _dx = (_wx2 - _wx1) / width;
            _dy = (_wy2 - _wy1) / height;

            using Image<Rgba32> render = new Image<Rgba32>(width, height);

            for (int y = 0; y < height; y++)
            {
                Span<Rgba32> pixelRowSpan = render.GetPixelRowSpan(y);

                for (int x = 0; x < width; x++)
                {
                    float distance = 0;
                    Vector3 color = Vector3.Zero;
                    Vector3 direction = new Vector3(_sx, _sy, 0) - _scene.CameraPosition;

                    Ray ray = new Ray(_scene.CameraPosition, Vector3.Normalize(direction));

                    Raytrace(ray, ref color, 1, 1f, ref distance);

                    int red = (int)Math.Clamp(color.X * 256, 0, 255);
                    int green = (int)Math.Clamp(color.Y * 256, 0, 255);
                    int blue = (int)Math.Clamp(color.Z * 256, 0, 255);

                    pixelRowSpan[x] = new Rgba32(red, green, blue);

                    _sx += _dx;
                }

                _sy += _dy;
            }

            return render.ToBase64String(PngFormat.Instance);
        }

        public void LoadScene(Scene scene)
        {
            _scene = scene;
        }

        private Primitive Raytrace(Ray ray, ref Vector3 color, int depth, float index, ref float distance)
        {
            if (depth > TRACE_DEPTH)
            {
                return null;
            }

            float minDistance = float.MaxValue;
            distance = 1000000f;

            Primitive closest = null;
            IntersectionResult result;

            foreach (var primitive in _scene.Primitives)
            {
                result = primitive.Intersects(ray, distance);

                if (result.RayIntersection == RayIntersection.Hit)
                {
                    if (result.Distance < minDistance)
                    {
                        closest = primitive;
                        minDistance = result.Distance;
                    }
                }
            }

            if (closest == null)
            {
                return null;
            }

            if (closest is Light)
            {
                color = Vector3.One;
            }
            else
            {
                Vector3 intersection = Vector3.Multiply(distance, Vector3.Add(ray.Origin, ray.Direction));

                foreach (var primitive in _scene.Primitives)
                {
                    if (primitive is Light)
                    {
                        Vector3 l = Vector3.Normalize(((Light)primitive).Center);
                        Vector3 n = closest.GetNormal(intersection);

                        if (closest.Material.Diffuse > 0)
                        {
                            float dot = Vector3.Dot(n, l);

                            if (dot > 0)
                            {
                                float diff = dot * closest.Material.Diffuse;
                                color += diff * closest.Material.Color * primitive.Material.Color;
                            }
                        }
                    }
                }
            }

            return closest;
        }
    }
}