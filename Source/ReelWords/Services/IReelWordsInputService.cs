namespace ReelWords.Services;

public interface IReelWordsInputService
{
    string InputWord();
    string InputUserName();
    bool CheckIfUserWantsLoadGame();
    bool CheckIfUserWantsSaveGame();
}
