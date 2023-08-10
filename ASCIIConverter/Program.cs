namespace ASCIIConverter;
using SkiaSharp;
class Program
{
    static string path = "/Users/drevoksi/Desktop/Crewmate.png";
    static string resultPath = "/Users/drevoksi/Desktop/CrewmateASCII.txt";
    static int newWidth = 60;
    static int newHeight = 30;
    static void Main(string[] args)
    {
        ASCIIFromPNG();
    }
    static void ASCIIFromPNG()
    {
        string weightsPath = "/Users/drevoksi/Desktop/Characters.txt";
        ASCIITable table = ASCIIConverter.TableFromWeights(weightsPath);
        ASCIIConverter converter = new ASCIIConverter(table);
        converter.ASCIIFromPNG(newWidth, newHeight, path, resultPath);
    }
    static void GreyscaleConvert()
    {
        PNGBytes.ConvertToGreyscale(newWidth, newHeight, path, resultPath);
    }
    static void SaveCharacterProperties()
    {
        string characters = """ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@₴£$%^&*_-=<>(){}[];:'"\|/,.`~§+ """;
        string path = "/Users/drevoksi/Desktop/Characters.png";
        int rowLength = 26;
        string resultPath = "/Users/drevoksi/Desktop/Characters.txt";
        Greyscale characterImageGreyscale = PNGBytes.GreyscaleFromPng(78, 9, path);
        ASCIIConverter.WriteCharacterWeights(characters.ToCharArray(), characterImageGreyscale, rowLength, resultPath);
    }
}
