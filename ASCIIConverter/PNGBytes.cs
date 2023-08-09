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
        public static void ConvertToGreyscale(int newWidth, int newHeight, string path, string resultPath) => SavePNG(resultPath, new Greyscale(BitmapFromPNG(path), newWidth, newHeight, false).GetBitmap());
        public static Greyscale GreyscaleFromPng(int newWidth, int newHeight, string path) => new Greyscale(BitmapFromPNG(path), newWidth, newHeight, false);
    }
}

