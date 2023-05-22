using System;
using ReelWords.Commands.Implementations;

namespace ReelWords.Commands;

public class UnexpectedErrorCommand : IUserGameCommand
{
    public Exception Error { get; }

    public UnexpectedErrorCommand(Exception error)
    {
        Error = error;
    }
}
