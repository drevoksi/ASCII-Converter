using System;
using SkiaSharp;
using System.IO;
namespace ASCIIConverter
{
    public static class PNGBytes
    {
        public static SKBitmap BitmapFromPNG(string path) => SKBitmap.Decode(path);
        public static byte[] PNGFromBitmap(SKBitmap bitmap) => bitmap.Encode(SKEncodedImageFormat.Png, 100).ToArray();
        public static void SavePNG(string path, SKBitmap bitmap) => File.WriteAllBytes(path, PNGFromBitmap(bitmap));
    }
}

