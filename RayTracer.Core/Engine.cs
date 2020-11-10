using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Numerics;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

using RayTracer.Core.Scenes;

namespace RayTracer.Core
{
    public sealed class Engine
    {
        private Scene _scene;

        public string Render(int width, int height)
        {
            if (_scene == null)
            {
                return null;
            }

            using Image<Rgba32> render = new Image<Rgba32>(400, 400);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    render[x, y] = new Rgba32(Vector3.Zero);
                }
            }

            var data = render
                .GetPixelMemoryGroup()
                .ToArray()[0];

            var bytes = MemoryMarshal.AsBytes(data.Span).ToArray();

            return Convert.ToBase64String(bytes);
        }

        public void LoadScene(Scene scene)
        {
            _scene = scene;
        }
    }
}