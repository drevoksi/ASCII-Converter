using System;
using SkiaSharp;
namespace ASCIIConverter
{
    public class Greyscale
    {
        public readonly int width;
        public readonly int height;
        readonly float[,] image;

        /// <summary>
        /// Create new Greyscales with resolution which is downscale only
        /// </summary>
        public Greyscale(SKBitmap bitmap, int newWidth, int newHeight, bool isGreyscale)
        {
            width = newWidth;
            height = newHeight;
            image = new float[width, height];
            int[,] samples = new int[width, height];
            float kx = (float)(width - 1) / (bitmap.Width - 1);
            float ky = (float)(height - 1) / (bitmap.Height - 1);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    int imageX = RoundToInt(x * kx);
                    int imageY = RoundToInt(y * ky);
                    image[imageX, imageY] += (isGreyscale ? bitmap.GetPixel(x, y).Red : ColorToGrey(bitmap.GetPixel(x, y))) / 255f;
                    samples[imageX, imageY]++;
                }
            }
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (samples[x, y] != 0)
                        image[x, y] = image[x, y] / samples[x, y];
        }
        public float this[int x, int y]
        {
            get { return image[x, y]; }
        }
        public SKBitmap GetBitmap()
        {
            SKBitmap bitmap = new SKBitmap(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    byte value = (byte)RoundToInt(image[x, y] * 255f);
                    bitmap.SetPixel(x, y, new SKColor(value, value, value));
                }
            }
            return bitmap;
        }
        private static int RoundToInt(float f) => Convert.ToInt32(Math.Round(f));
        private static float ColorToGrey(SKColor color) => 0.3f * color.Red + 0.58f * color.Green + 0.12f * color.Blue;
    }
}

