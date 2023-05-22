namespace ReelWords.Domain.ValueObjects;

public class Word
{
    public string Value { get; private set; }

    public int Score { get; private set; }

    public static Word Create(string value, int score)
        => new Word() { Value = value, Score = score };

    public override bool Equals(object? obj) 
        => obj is Word word && word.Value == Value && word.Score == Score;
}
