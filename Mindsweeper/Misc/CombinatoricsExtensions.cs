namespace Mindsweeper.Misc
{
    public static class CombinatoricsExtensions
    {
        public static IEnumerable<IEnumerable<T>> AsCombinations<T>(this IEnumerable<T> source, int requiredCount)
        {
            var list = source.ToList();
            int count = list.Count;

            // need to make this a double if over 32 items but that's not possible here since minesweeper can only have 8
            for (int i = 0; i < 1 << count; i++)
            {
                // if number of flags in bitmask is not requiredCount then no need to return that combination
                if (CountBits(i) != requiredCount) continue;
                yield return GetCombination(list, i);
            }
        }

        private static int CountBits(int bitmask)
        {
            int total = 0;
            while (bitmask > 0)
            {
                if ((bitmask & 1) == 1)
                {
                    total++;
                }
                bitmask >>= 1;
            }
            return total;
        }

        private static IEnumerable<T> GetCombination<T>(List<T> list, int bitmask)
        {
            int index = 0;
            while (bitmask > 0)
            {
                if ((bitmask & 1) == 1)
                {
                    yield return list[index];
                }
                bitmask >>= 1;
                index++;
            }
        }
    }
}
