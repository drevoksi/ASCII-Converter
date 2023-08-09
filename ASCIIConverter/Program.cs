namespace ASCIIConverter;
using SkiaSharp;
class Program
{
    static void Main(string[] args)
    {
        string path = "/Users/drevoksi/Desktop/ColourSpace.png";
        string resultPath = "/Users/drevoksi/Desktop/ColourSpaceGreyscale.png";
        SKBitmap bitmap = PNGBytes.BitmapFromPNG(path);
        Greyscale greyscale = new Greyscale(bitmap, bitmap.Width, bitmap.Height, false);
        PNGBytes.SavePNG(resultPath, greyscale.GetBitmap());
    }
}
