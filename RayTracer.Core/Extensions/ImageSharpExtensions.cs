using System.Drawing;
using System.IO;
using System.Runtime.Versioning;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace RayTracer.Core.Extensions;

public static class ImageSharpExtensions
{
    [SupportedOSPlatform("windows")]
    public static Bitmap ToBitmap<TPixel>(this Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
    {
        using var memoryStream = new MemoryStream();

        image.Save(memoryStream, new PngEncoder());
        memoryStream.Seek(0, SeekOrigin.Begin);

        return new Bitmap(memoryStream);
    }

    [SupportedOSPlatform("windows")]
    public static Image<TPixel> ToImageSharpImage<TPixel>(this System.Drawing.Bitmap bitmap) where TPixel : unmanaged, IPixel<TPixel>
    {
        using var memoryStream = new MemoryStream();

        bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
        memoryStream.Seek(0, SeekOrigin.Begin);

        return SixLabors.ImageSharp.Image.Load<TPixel>(memoryStream);
    }
}