using System;
namespace ASCIIConverter
{
    public class ASCIIConverter
    {
        public readonly ASCIITable table;
        public ASCIIConverter(ASCIITable newTable)
        {
            table = newTable;
        }
    }
    public class ASCIITable
    {
        public class Weight
        {
            public static float DistanceSquared(Weight a, Weight b)
            {
                float? minDistance = null;
                List<Weight> firstList = new() { a }, secondList = new() { b };
                if (a.inverse != null) firstList.Add(a.inverse);
                if (b.inverse != null) secondList.Add(b.inverse);
                foreach (Weight first in firstList)
                {
                    foreach (Weight second in secondList)
                    {
                        float dx = second.x - first.x, dy = second.y - first.x;
                        float distanceSquared = dx * dx + dy * dy;
                        if (minDistance == null || distanceSquared < minDistance)
                            minDistance = distanceSquared;
                    }
                }
                return (float)minDistance;
            }
            public float x;
            public float y;
            public Weight? inverse;
            public Weight(float newX, float newY, bool isMirrored)
            {
                x = newX;
                y = newY;
                if (isMirrored) inverse = new Weight(-newX, -newY, false);
            }
        }
        public class CharacterSet
        {
            public int value;
            public (char, Weight)[] characters;
            public CharacterSet(int newValue, (char, Weight)[] newCharacters)
            {
                value = newValue;
                characters = newCharacters;
            }
        }
        private CharacterSet[] table;
        public ASCIITable(CharacterSet[] characterSets)
        {

        }
    }
}

