using System.Numerics;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RayTracer.Core.Materials;

public class Texture(string filePath)
{
    private readonly Image<Rgba32> _image = Image.Load<Rgba32>(filePath);

    public Vector3 Sample(float u, float v)
    {
        u -= System.MathF.Floor(u);
        v -= System.MathF.Floor(v);
        int x = System.Math.Min((int)(_image.Width * u), _image.Width - 1);
        int y = System.Math.Min((int)(_image.Height * v), _image.Height - 1);

        Rgba32 pixel = _image[x, y];

        return new Vector3(pixel.R / 255f, pixel.G / 255f, pixel.B / 255f);
    }

    public Vector3 SampleTriplanar(Vector3 position, Vector3 normal)
    {
        Vector3 weights = Vector3.Abs(normal);
        weights *= weights;
        weights *= weights;
        float sum = weights.X + weights.Y + weights.Z;
        weights = sum > 0 ? weights / sum : Vector3.UnitZ;
        return weights.X * SampleBilinear(position.Z, position.Y)
            + weights.Y * SampleBilinear(position.X, position.Z)
            + weights.Z * SampleBilinear(position.X, position.Y);
    }

    private Vector3 SampleBilinear(float u, float v)
    {
        float x = (u - System.MathF.Floor(u)) * _image.Width - .5f;
        float y = (v - System.MathF.Floor(v)) * _image.Height - .5f;
        int ix = (int)System.MathF.Floor(x), iy = (int)System.MathF.Floor(y);
        Vector3 Pixel(int px, int py)
        {
            Rgba32 c = _image[(px + _image.Width) % _image.Width, (py + _image.Height) % _image.Height];
            return new(c.R / 255f, c.G / 255f, c.B / 255f);
        }
        return Vector3.Lerp(Vector3.Lerp(Pixel(ix, iy), Pixel(ix + 1, iy), x - ix),
            Vector3.Lerp(Pixel(ix, iy + 1), Pixel(ix + 1, iy + 1), x - ix), y - iy);
    }
}
