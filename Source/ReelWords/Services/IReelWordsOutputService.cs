using ReelWords.Domain.Entities;
using ReelWords.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReelWords.Services;

public interface IReelWordsOutputService
{
    void ShowWrongInput(string message);
    Task ShowUnexpectedError(string message);
    void ShowWelcome(int penalty);
    void ShowHelp(int penalty);
    void ShowGoodbye();
    void ShowLevelAndScore(int level, int score);
    void ShowReelLetters(ReelPanel reelPanel, Dictionary<char, int> scores);
    void ShowSaveGameResponse(bool isOk);
    void ShowWords(List<Word> words);
    void ShowPenalty(int points, int totalScore);
    void ShowWordSubmitted(string word, int points, int totalScore);
}
