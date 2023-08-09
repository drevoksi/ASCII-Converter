namespace ASCIIConverter;
using SkiaSharp;
class Program
{
    static void Main(string[] args)
    {
        SaveCharacterProperties();
    }
    static void SaveCharacterProperties()
    {
        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@₴£$%^&*_-=<>(){}[];:'\"\\|/,.`~§+ ";
        string characterImagePath = "/Users/drevoksi/Desktop/Characters.png";
        int rowLength = 26;
        string resultPath = "/Users/drevoksi/Desktop/Characters.txt";
        Console.WriteLine(characters.Length);
        Greyscale characterImageGreyscale = PNGBytes.GreyscaleFromPng(78, 9, characterImagePath);
        ASCIIConverter.WriteCharacterWeights(characters.ToCharArray(), characterImageGreyscale, rowLength, resultPath);
    }
    static void GreyscaleConvert()
    {
        string path = "/Users/drevoksi/Desktop/Characters.png";
        PNGBytes.ConvertToGreyscale(78, 9, path, "/Users/drevoksi/Desktop/Greyscale.png");
    }
}
