using ReelWords.Commands.Implementations;

namespace ReelWords.Commands;

public class ExitGameCommand : IUserGameCommand
{
    public bool SaveGame { get; }

    public ExitGameCommand(bool saveGame)
    {
        SaveGame = saveGame;
    }
}
