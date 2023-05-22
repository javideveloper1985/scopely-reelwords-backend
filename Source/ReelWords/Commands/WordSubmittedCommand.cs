using ReelWords.Commands.Implementations;

namespace ReelWords.Commands;

public class WordSubmittedCommand : IUserGameCommand
{
    public string Value { get; }
    public int Score { get; }
    public WordSubmittedCommand(string value, int score)
    {
        Value = value;
        Score = score;
    }
}
