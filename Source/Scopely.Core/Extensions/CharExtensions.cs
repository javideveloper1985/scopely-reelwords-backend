namespace Scopely.Core.Extensions;

public static class CharExtensions
{
    public static int RemoveLastFoundItem<ItemType>(this ItemType[] array, ItemType itemToRemove)
    {
        var lastIndex = Array.LastIndexOf(array, itemToRemove);

        if (lastIndex > -1)
        {
            for (int i = lastIndex; i < array.Length - 1; i++)
                array[i] = array[i + 1];

            Array.Resize(ref array, array.Length - 1);
            return lastIndex;
        }

        return lastIndex;
    }

    public static List<int> GetLastCharIndexesOfWord(this char[] array, string word)
    {
        var colIndexes = new List<int>();
        foreach (var letter in word)
        {
            for (int arrayIdx = array.Length - 1; arrayIdx >= 0; arrayIdx--)
            {
                var letterFound = char.ToLower(array[arrayIdx]) == char.ToLower(letter);
                if (letterFound && !colIndexes.Contains(arrayIdx))
                {
                    colIndexes.Add(arrayIdx);
                    break;
                }
            }
        }
        return colIndexes;
    }
}
