using ReelWords.Commands.Implementations;

namespace ReelWords.Commands;

public class UserNameCommand : IUserGameCommand 
{
    public string UserName { get; }

    public UserNameCommand(string userName)
    {
        UserName = userName;
    }
}
