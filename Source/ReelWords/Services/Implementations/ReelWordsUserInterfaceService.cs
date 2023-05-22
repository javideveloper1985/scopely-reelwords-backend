using ReelWords.Constants;
using ReelWords.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReelWords.Services.Implementations;

public class ReelWordsUserInterfaceService : IReelWordsUserInterfaceService
{
    private readonly IConsoleUserInterfaceService _consoleUiService;

    public ReelWordsUserInterfaceService(IConsoleUserInterfaceService uiService)
    {
        _consoleUiService = uiService ?? throw new ArgumentNullException(nameof(uiService));
    }

    public void ShowReel(ReelPanel reelPanel, Dictionary<char, int> scores)
    {
        _consoleUiService.NewLine();
        var currentReel = reelPanel.GetCurrentReel();
        StringBuilder reelText = new();
        reelText.Append(" | ");
        foreach (var letter in currentReel)
        {
            var score = scores.ContainsKey(letter) ? scores[letter] : 0;
            reelText.Append($"{char.ToUpper(letter)} ({score})");
            reelText.Append(" | ");
        }
        _consoleUiService.ShowMessage(reelText.ToString(), ConsoleColor.Yellow);
    }

    public void ShowGoodbye()
    {
        _consoleUiService.NewLine();
        var sb = new StringBuilder();
        sb.AppendLine("********************************************");
        sb.AppendLine($"************ {Messages.Thanks} ************");
        sb.AppendLine("********************************************");
        _consoleUiService.ShowMessage(sb.ToString(), ConsoleColor.Cyan);
    }

    public void ShowLevelScore(int level, int score)
    {
        _consoleUiService.NewLine();
        _consoleUiService.ShowMessage($"********************", ConsoleColor.Blue);
        _consoleUiService.ShowMessage($"**** Level {level:000#} ****", ConsoleColor.Blue);
        _consoleUiService.ShowMessage($"**** Score {score:000#} ****", ConsoleColor.Blue);
        _consoleUiService.ShowMessage($"********************", ConsoleColor.Blue);
    }

    public void ShowWelcome(int penalty)
    {
        _consoleUiService.NewLine();

        var sb = new StringBuilder();
        sb.AppendLine("**********************************************");
        sb.AppendLine($"************ {Messages.Welcome} ************");
        sb.AppendLine("**********************************************");
        sb.AppendLine();
        _consoleUiService.ShowMessage(sb.ToString(), ConsoleColor.Cyan);

        _consoleUiService.ShowTitle("Game instructions:");
        sb.Clear();
        sb.AppendLine($" - {Messages.GameInstructions}");
        sb.AppendLine();
        _consoleUiService.ShowMessage(sb.ToString());

        _consoleUiService.ShowTitle("Special commands:");
        sb.Clear();
        sb.AppendLine($" - '{UserKeyWords.Exit}' -> End game.");
        sb.AppendLine($" - '{UserKeyWords.Shuffle}' -> If you cannot find words, you can shuffle the letters with a {penalty} points penalty. (Only during the game)");
        sb.AppendLine($" - '{UserKeyWords.ShowWords}' -> Show submitted words in the game. (Only during the game)");
        sb.AppendLine();
        _consoleUiService.ShowMessage(sb.ToString());
    }

    public string InputWord()
    {
        _consoleUiService.NewLine();
        return GetUserInput(Messages.EnterWord);
    }

    public string InputUserName()
    {
        _consoleUiService.NewLine();
        return GetUserInput(Messages.EnterUserName);
    }

    public bool CheckIfUserWantsLoadGame()
    {
        _consoleUiService.NewLine();
        _consoleUiService.ShowMessage("HEY!!", ConsoleColor.Yellow);

        var input = _consoleUiService
            .AskForInputOption($"There is a saved game. Would you like to continue that game?", "y", "n");

        return input == "y";
    }

    public bool CheckIfUserWantsSaveGame()
    {
        _consoleUiService.NewLine();
        var options = new string[] { "y", "n" };
        var input = _consoleUiService.AskForInputOption(Messages.AskSave, options);
        return input == "y";
    }

    public void ShowSaveGameResponse(bool isOk)
    {
        _consoleUiService.NewLine();
        if (!isOk)
            _consoleUiService.ShowError("The game couldn't be saved due to an error.");
        else
            _consoleUiService.ShowSuccess("Game saved!!");
    }

    public void ShowWords(List<Word> words)
    {
        var sb = new StringBuilder();
        foreach (var pw in words.OrderByDescending(w => w.Score))
            sb.AppendLine($" - {pw.Value} ({pw.Score})");

        _consoleUiService.NewLine();
        _consoleUiService.ShowMessage("Submitted words:", ConsoleColor.Yellow);
        _consoleUiService.ShowMessage(sb.ToString(), ConsoleColor.DarkGreen);
    }

    public void ShowWrongInput(string message)
    {
        _consoleUiService.ShowError(message);
    }

    public async Task ShowUnexpectedError(string message)
    {
        await Task.Run(() =>
        {
            _consoleUiService.ShowError(message);
            _consoleUiService.GetInput("Press enter to continue.");
        });
    }

    private string GetUserInput(string message = "")
    {
        string word;
        do
        {
            word = _consoleUiService.GetInput(message);
        }
        while (string.IsNullOrWhiteSpace(word));
        return word.ToLower();
    }

    public void ShowPenalty(int points, int totalScore)
    {
        _consoleUiService.NewLine();
        _consoleUiService.ShowError($"Oops!! You have lost {points} points. Total points: {totalScore}.");
    }

    public void ShowWordSubmitted(string word, int points, int totalScore)
    {
        _consoleUiService.NewLine();
        _consoleUiService.ShowSuccess($"Great!! The word '{word}' is correct. You have obtained {points} points!! Total points: {totalScore}.");
    }
}
