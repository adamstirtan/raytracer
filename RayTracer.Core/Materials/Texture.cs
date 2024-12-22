using System.Numerics;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RayTracer.Core.Materials;

public class Texture(string filePath)
{
    private readonly Image<Rgba32> _image = Image.Load<Rgba32>(filePath);

    public Vector3 Sample(float u, float v)
    {
        int x = (int)(_image.Width * u) % _image.Width;
        int y = (int)(_image.Height * v) % _image.Height;

        Rgba32 pixel = _image[x, y];

        return new Vector3(pixel.R / 255f, pixel.G / 255f, pixel.B / 255f);
    }
}
