using ReelWords.Commands.Implementations;

namespace ReelWords.Commands;

public class InvalidWordCommand : IUserGameCommand
{
    public string Message { get; }
    public InvalidWordCommand(string message)
    {
        Message = message;
    }
}
