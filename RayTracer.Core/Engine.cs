using System;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
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

            using Image<Rgba32> render = new Image<Rgba32>(width, height);

            for (int y = 0; y < height; y++)
            {
                Span<Rgba32> pixelRowSpan = render.GetPixelRowSpan(y);

                for (int x = 0; x < render.Width; x++)
                {
                    pixelRowSpan[x] = new Rgba32(120, 120, 120);
                }
            }

            return render.ToBase64String(PngFormat.Instance);
        }

        public void LoadScene(Scene scene)
        {
            _scene = scene;
        }
    }
}