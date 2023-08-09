using System;
using System.IO;
namespace ASCIIConverter
{
    public class ASCIIConverter
    {
        /// <summary>
        /// Each character in the image must be a 3*3 square.
        /// They are read row by row from left to right.
        /// There must not be any space between them. \n
        /// Note: colours are reversed - white is minimum weight.
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
                        weights[x + y * 3] = 1 - greyscale[charX + x, charY + y];
                float average = weights.Sum() / weights.Length;
                lines.Add((RoundToInt(average * 63.75f) * 4).ToString());
                foreach (float weight in weights)
                {
                    lines.Add(weight.ToString());
                }
            }
            File.WriteAllLines(resultPath, lines);
        }
        public readonly ASCIITable table;
        public ASCIIConverter(ASCIITable newTable)
        {
            table = newTable;
        }
        private static int RoundToInt(float f) => Convert.ToInt32(Math.Round(f));
    }
    public class ASCIITable
    {
        public struct WeightSet
        {
            public const int length = 9;
            public float DifferenceSquaredSum(WeightSet a, WeightSet b)
            {
                float result = 0;
                for (int i = 0; i < length; i++)
                {
                    float d = b[i] - a[i];
                    result += d * d;
                }
                return result;
            }
            float[] weights;
            public WeightSet(float[] newWeights)
            {
                weights = new float[length];
                for (int i = 0; i < Math.Min(length, newWeights.Length); i++)
                    weights[i] = newWeights[i];
            }
            public readonly float this[int index]
            {
                get { return weights[index]; }
            }
        }
        public class CharacterSet
        {
            public int value;
            public (char, WeightSet)[] characters;
            public CharacterSet(int newValue, (char, WeightSet)[] newCharacters)
            {
                value = newValue;
                characters = newCharacters;
            }
        }
        private CharacterSet[] table;
        public ASCIITable(CharacterSet[] characterSets)
        {
            characterSets.OrderBy(x => x.value);
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
    }
}

