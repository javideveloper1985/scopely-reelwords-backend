namespace ReelWords.Domain.ValueObjects;

public class Letter
{
    public char Value { get; private set; }
    public int Score { get; private set; }

    public Letter(char value, int score)
    {
        Value = value;
        Score = score;
    }
}
