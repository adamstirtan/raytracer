using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;

using Newtonsoft.Json;

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

            var render = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    render.SetPixel(x, y, Color.Black);
                }
            }

            return render;
        }

        public void LoadScene(Scene scene)
        {
            _scene = scene;
        }

        public async Task LoadSceneAsync(string fileName)
        {
            var directory = GetConfigurationDirectory();
            var filePath = Path.Combine(directory, fileName);

            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"Configuration file not found: {filePath}");
            }

            var configuration = JsonConvert.DeserializeObject<Configuration>(
                await File.ReadAllTextAsync(filePath));

            _scene = new Scene();

            foreach (var p in configuration.Primitives)
            {
                _scene.AddPrimitive(p);
            }
        }

        private static string GetConfigurationDirectory()
        {
            var baseDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            if (baseDirectory.Parent?.Parent?.Parent != null)
            {
                return baseDirectory.Parent.Parent.Parent.FullName;
            }

            throw new DirectoryNotFoundException();
        }
    }
}