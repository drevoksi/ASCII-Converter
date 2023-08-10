using System;
using System.IO;
namespace ASCIIConverter
{
    public class ASCIIConverter
    {
        /// <summary>
        /// Each character in the image must be a 3*3 square.
        /// They are read row by row from left to right.
        /// There must not be any space between them.
        /// Note: white is maximum weight, weights are relative to the centre square.
        /// </summary>
        public static void WriteCharacterWeights(char[] chars, Greyscale greyscale, int rowLength, string resultPath)
        {
            List<string> lines = new List<string>();
            for (int i = 0; i < chars.Length; i++)
            {
                lines.Add("" + chars[i]);
                int charX = (i % rowLength) * 3;
                int charY = (i / rowLength) * 3;
                float[] weights = new float[9];
                for (int y = 0; y < 3; y++)
                    for (int x = 0; x < 3; x++)
                        weights[x + y * 3] = greyscale[charX + x, charY + y];
                float average = weights.Sum() / weights.Length;
                lines.Add((RoundToInt(average * 63.75f) * 4).ToString());
                float centreWeight = weights[4];
                for (int w = 0; w < weights.Length; w++)
                {
                    if (w == 4) continue;
                    lines.Add((weights[w] - centreWeight).ToString());
                }
            }
            File.WriteAllLines(resultPath, lines);
        }
        /// <summary>
        /// Converts generated character weight text file to character array.
        /// In the file, there must be 10 lines per character: [0] is the character, [1] is value and [2 - 9] are weights.
        /// </summary>
        public static ASCIITable.Character[] CharactersFromWeights(string path)
        {
            Queue<string> lines = new Queue<string>(File.ReadAllLines(path));
            List<ASCIITable.Character> characters = new List<ASCIITable.Character>();
            while (lines.Count > 0)
            {
                char character = lines.Dequeue()[0];
                int value = Convert.ToInt32(lines.Dequeue());
                float[] weights = new float[8];
                for (int i = 0; i < weights.Length; i++)
                    weights[i] = Convert.ToSingle(lines.Dequeue());
                characters.Add(new ASCIITable.Character(character, value, weights));
            }
            return characters.ToArray();
        }
        public static ASCIITable.CharacterSet[] CharacterSetsFromCharacters(ASCIITable.Character[] characters)
        {
            characters = characters.OrderBy(x => x.value).ToArray();
            Queue<ASCIITable.Character> queue = new Queue<ASCIITable.Character>(characters);
            List<ASCIITable.CharacterSet> characterSets = new List<ASCIITable.CharacterSet>()
                { new ASCIITable.CharacterSet(queue.Peek().value) };
            while (queue.Count > 0)
            {
                ASCIITable.Character character = queue.Dequeue();
                if (character.value != characterSets.Last().value)
                    characterSets.Add(new ASCIITable.CharacterSet(character.value));
                characterSets.Last().Add(character);
            }
            return characterSets.ToArray();
        }
        public static ASCIITable TableFromWeights(string path)
        {
            ASCIITable.Character[] characters = CharactersFromWeights(path);
            ASCIITable.CharacterSet[] characterSets = CharacterSetsFromCharacters(characters);
            return new ASCIITable(characterSets);
        }
        public readonly ASCIITable table;
        public ASCIIConverter(ASCIITable newTable)
        {
            table = newTable;
        }
        public void ASCIIFromPNG(int width, int height, string path, string resultPath, bool singleLine = false)
        {
            string ASCII = ASCIIFromPNG(width, height, path);
            List<string> lines = new List<string>();
            if (!singleLine)
            {
                Queue<char> queue = new Queue<char>(ASCII);
                for (int y = 0; y < height; y++)
                {
                    string line = "";
                    for (int x = 0; x < width; x++)
                        line += queue.Dequeue();
                    lines.Add(line);
                }
            }
            else lines.Add(ASCII);
            File.WriteAllLines(resultPath, lines);
        }
        public string ASCIIFromPNG(int width, int height, string path)
        {
            Greyscale image = PNGBytes.GreyscaleFromPng(width, height, path);
            string result = "";
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float centre = image[x, y];
                    int value = RoundToInt(centre * 255);
                    float[] relativeWeights = new float[8];
                    int[] c = { -1, 0, 1 };
                    int i = 0;
                    foreach (int dx in c)
                    {
                        foreach (int dy in c)
                        {
                            if (dx == 0 && dy == 0) continue;
                            int wx = x + dx;
                            int wy = y + dy;
                            float relativeWeight;
                            if ((wx < 0 || wx >= image.width) || (wy < 0 || wy >= image.height))
                                relativeWeight = 0;
                            else relativeWeight = image[wx, wy];
                            relativeWeight -= centre;
                            relativeWeights[i] = relativeWeight;
                            i++;
                        }
                    }
                    result += table.GetCharFor(value, relativeWeights);
                }
            }
            return result;
        }
        private static int RoundToInt(float f) => Convert.ToInt32(Math.Round(f));
    }
    public class ASCIITable
    {
        public struct Character
        {
            public static float WeightDifferenceSquared(float[] a, float[] b)
            {
                float result = 0;
                for (int i = 0; i < 8; i++)
                {
                    float d = b[i] - a[i];
                    result += d * d;
                }
                return result;
            }
            public readonly char character;
            public readonly int value;
            public readonly float[] weights;
            public Character(char newCharacter, int newValue, float[] newWeights)
            {
                character = newCharacter;
                value = newValue;
                weights = new float[8];
                for (int i = 0; i < Math.Min(8, newWeights.Length); i++)
                    weights[i] = newWeights[i];
            }
        }
        public class CharacterSet
        {
            public readonly int value;
            public int Count { get { return characters.Count; } }
            public List<Character> characters;
            public CharacterSet(int newValue)
            {
                value = newValue;
                characters = new List<Character>();
            }
            public void Add(Character character)
            {
                characters.Add(character);
            }
        }
        private CharacterSet[] table;
        public ASCIITable(CharacterSet[] characterSets)
        {
            characterSets = characterSets.OrderBy(x => x.value).ToArray();
            int maxValue = characterSets.Last().value;
            table = new CharacterSet[maxValue + 1];
            Queue<CharacterSet> queue = new Queue<CharacterSet>(characterSets);
            CharacterSet currentSet = queue.Dequeue();
            int currentLength = 0;
            for (int i = 0; i < table.Length; i++)
            {
                int nextSetIndex = queue.Peek().value;
                if (i == nextSetIndex)
                {
                    currentSet = queue.Dequeue();
                    currentLength = 0;
                }
                table[i] = currentLength <= (nextSetIndex - i) * 0.5f ? currentSet : queue.Peek();
                currentLength++;
            }
        }
        public char GetCharFor(int value, float[] weights)
        {
            List<Character> characters = table[value].characters;
            (char, float)[] charWeightDifferences =
                characters.Select(x => (x.character, Character.WeightDifferenceSquared(x.weights, weights))).ToArray();
            charWeightDifferences = charWeightDifferences.OrderBy(x => x.Item2).ToArray();
            return charWeightDifferences[0].Item1;
        }
    }
}

