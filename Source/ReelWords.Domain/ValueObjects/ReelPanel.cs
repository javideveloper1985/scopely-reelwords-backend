using Scopely.Core.Extensions;
using Scopely.Core.Structures;
using System.Text;

namespace ReelWords.Domain.ValueObjects;

public class ReelPanel
{
    public Matrix<char> Matrix { get; private set; }

    public int RowCount => Matrix.GetLength(0);

    public int ColumnCount => Matrix.GetLength(1);

    private ReelPanel(int rows, int columns)
        => Matrix = new Matrix<char>(rows, columns);

    public static ReelPanel CreateEmpty(int rows, int columns)
        => new(rows, columns);

    public void AddReel(int row, char[] chars)
    {
        if (chars is null || chars.Length != Matrix.GetLength(1))
            return;

        for (int i = 0; i < chars.Length; i++)
            Matrix[row, i] = char.ToLower(chars[i]);
    }

    public void Shuffle() => Matrix.Shuffle();

    /// <summary>
    /// Check if a word can be formed with the letters in the current reel
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public bool CheckWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return false;

        var reelLetters = new char[ColumnCount];
        for (int i = 0; i < ColumnCount; i++)
            reelLetters[i] = Matrix[RowCount - 1, i];

        foreach (var letter in word)
        {
            if (CharExtensions.RemoveLastFoundItem(ref reelLetters, letter) == -1)
                return false;
        }

        return true;
    }

    public char[] GetCurrentReel() => GetReelByRow(RowCount - 1);

    public void ScrollLetters(string word)
    {
        var currentReel = GetCurrentReel();
        var colIdxToScroll = currentReel.GetLastCharIndexesOfWord(word);
        foreach (var colToScroll in colIdxToScroll)
        {
            var lastElement = Matrix[Matrix.GetLength(0) - 1, colToScroll];
            for (int row = Matrix.GetLength(0) - 1; row > 0; row--)
                Matrix[row, colToScroll] = Matrix[row - 1, colToScroll];
            Matrix[0, colToScroll] = lastElement;
        }
    }

    public char[] GetReelByRow(int row)
    {
        var reelLetters = new char[ColumnCount];
        for (int i = 0; i < ColumnCount; i++)
            reelLetters[i] = Matrix[row, i];
        return reelLetters;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int row = 0; row < Matrix.GetLength(0); row++)
        {
            var letters = GetReelByRow(row);
            var line = string.Join("|", letters);
            sb.AppendLine(line);
        }
        return sb.ToString();
    }
}
