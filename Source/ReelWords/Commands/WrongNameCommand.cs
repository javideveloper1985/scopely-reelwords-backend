using ReelWords.Commands.Implementations;

namespace ReelWords.Commands;

public class WrongNameCommand : IUserGameCommand 
{
    public string Message { get; set; }

    public WrongNameCommand(string message)
    {
        Message = message;
    }
}
