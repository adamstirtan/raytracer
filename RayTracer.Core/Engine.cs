using SixLabors.ImageSharp;

using RayTracer.Core.Scenes;

namespace RayTracer.Core
{
    public sealed class Engine
    {
        private Scene _scene;

        public Image Render(int width, int height)
        {
            if (_scene == null)
            {
                return null;
            }

            //var bitmap = new Image(SixLabors.ImageSharp.Configuration.Default, BmpBitsPerPixel, new Size(width, height))

            //for (int y = 0; y < height; y++)
            //{
            //    for (int x = 0; x < width; x++)
            //    {
            //        bitmap.SetPixel(x, y, Color.Black);
            //    }
            //}

            return null;
        }

        public void LoadScene(Scene scene)
        {
            _scene = scene;
        }
    }
}