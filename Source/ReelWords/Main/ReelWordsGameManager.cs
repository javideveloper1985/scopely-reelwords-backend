using Microsoft.Extensions.Configuration;
using ReelWords.Constants;
using ReelWords.Domain.Entities;
using ReelWords.Domain.Factories;
using ReelWords.Domain.Services;
using ReelWords.Domain.ValueObjects;
using ReelWords.Services;
using ReelWords.UseCases;
using Scopely.Core.Enums;
using Scopely.Core.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReelWords.Main;

public class ReelWordsGameManager : IReelWordsGameManager
{
    private readonly ILoadGameUseCase _loadGame;
    private readonly ICreateGameUseCase _createGame;
    private readonly ISaveGameUseCase _saveGame;

    private readonly IGetLetterScoresService _calculateScoreService;
    private readonly IGetDictionaryService _getValidWordsService;
    private readonly IConsoleUserInterfaceService _uiService;

    private Game _game;
    private Dictionary<char, int> _charScores;
    private Trie _validWordsTrie;
    private string _validChars;

    private bool _stopGame = false;

    //TODO: Better obtain these values from settings
    private readonly int _defaultWordSize;
    private readonly int _shufflePenaltyPoints;

    //TODO: Inject ILogger to create system logs
    public ReelWordsGameManager(
        ILoadGameUseCase loadGame,
        ICreateGameUseCase createGame,
        ISaveGameUseCase saveGame,
        IGetLetterScoresService scoreService,
        IGetDictionaryService getDictionaryService,
        IConsoleUserInterfaceService userInterface,
        IConfiguration config)
    {
        _loadGame = loadGame ?? throw new ArgumentNullException(nameof(loadGame));
        _createGame = createGame ?? throw new ArgumentNullException(nameof(createGame));
        _saveGame = saveGame ?? throw new ArgumentNullException(nameof(saveGame));
        _calculateScoreService = scoreService ?? throw new ArgumentNullException(nameof(scoreService));
        _getValidWordsService = getDictionaryService ?? throw new ArgumentNullException(nameof(getDictionaryService));
        _uiService = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
        if (config is null)
            throw new ArgumentNullException(nameof(config));

        _defaultWordSize = int.TryParse(config[ConfigKeys.WordSize], out int valueSize) ? valueSize : 7;
        _shufflePenaltyPoints = int.TryParse(config[ConfigKeys.ShufflePenalty], out int valuePenalty) ? valuePenalty : 2;
    }

    private async Task LoadGameData()
    {
        _charScores = await _calculateScoreService.Get();
        var validWords = await _getValidWordsService.GetByWordSize(_defaultWordSize);
        _validWordsTrie = Trie.CreateFromListOfWords(validWords);
        _validChars = ValidCharsFactory.Get(Language.English);
    }

    public async Task Start()
    {
        try
        {
            Welcome();

            await LoadGameData();

            var userId = GetUserInput(Messages.EnterUser);
            await CheckExitKeyWord(userId, true);
            if (_stopGame)
                return;

            _game = await GetSavedGame(userId);
            if (_stopGame)
                return;

            _game ??= await CreateNewGame(userId);
            if (_stopGame)
                return;

            _uiService.NewLine();
            _uiService.ShowSuccess($" *** HI {userId} ***");
            _uiService.ShowSuccess($" - Current score: {_game.Score} ***");
            _uiService.NewLine();

            do
            {
                await StartRound();
            }
            while (!_stopGame);

        }
        catch (Exception ex)
        {
            //TODO: Log the exception when the ILogger has been injected
            Quit(Messages.UnexpectedError);
        }
    }

    private async Task StartRound()
    {
        var wordPoints = -1;
        string word;

        do
        {
            _uiService.NewLine();

            PrintReel();

            word = GetUserInput(Messages.EnterWord);
            await CheckExitKeyWord(word, false);
            if (_stopGame)
                return;

            if (!CheckShuffleKeyWord(word))
            {
                var isValidWord = ValidateWordScore(word);
                if (isValidWord)
                    wordPoints = ApplyWordScore(word);
            }

        } while (wordPoints == -1);
    }

    private async Task CheckExitKeyWord(string word, bool forceExit)
    {
        if (word.Equals(UserKeyWords.Exit, StringComparison.InvariantCultureIgnoreCase))
        {
            if (!forceExit && AskSaveGame())
                await Save();

            Quit();
        }
    }

    private bool CheckShuffleKeyWord(string word)
    {
        if (word.Equals(UserKeyWords.Shuffle, StringComparison.InvariantCultureIgnoreCase))
        {
            _game.ReelPanel.Shuffle();
            _game.AddScore(_shufflePenaltyPoints * -1);
            _uiService.ShowError($"You have lost {_shufflePenaltyPoints} points. Total points: {_game.Score}");
            return true;
        }

        return false;
    }

    private async Task<Game> GetSavedGame(string userId)
    {
        var gameResponse = await _loadGame.Execute(userId);
        if (!gameResponse.IsOk)
            Quit("Error loading saved game.");

        if (gameResponse.Value is not null)
        {
            _uiService.ShowMessage("HEY!", ConsoleColor.Yellow);
            var input = _uiService.AskForInputOption($"There is a saved game for the user {userId}. " +
                $"Would you like to continue that game?", "y", "n");

            return input == "y" ? gameResponse.Value : null;
        }

        return null;
    }

    private async Task<Game> CreateNewGame(string userId)
    {
        var gameResponse = await _createGame.Execute(_defaultWordSize, userId);
        if (!gameResponse.IsOk)
            Quit("Error creating new game.");

        return gameResponse.Value;
    }

    private void PrintReel()
    {
        var currentReel = _game.ReelPanel.GetCurrentReel();
        StringBuilder reelText = new();
        reelText.Append(" | ");
        foreach (var letter in currentReel)
        {
            var score = _charScores.ContainsKey(letter) ? _charScores[letter] : 0;
            reelText.Append($"{char.ToUpper(letter)} ({score})");
            reelText.Append(" | ");
        }

        _uiService.ShowMessage(reelText.ToString(), ConsoleColor.Yellow);
        _uiService.NewLine();
    }

    private string GetUserInput(string message = "")
    {
        string word;
        do
        {
            word = _uiService.GetInput(message);
        }
        while (string.IsNullOrWhiteSpace(word));

        _uiService.NewLine();

        return word.ToLower();
    }

    private bool ValidateWordScore(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return false;

        foreach (var letter in word)
        {
            if (!_validChars.Contains(letter))
            {
                _uiService.ShowError(Messages.WrongWordLanguage);
                return false;
            }
        }

        if (!_game.ReelPanel.CheckWord(word))
        {
            _uiService.ShowError(Messages.WrongWordReel);
            return false;
        }

        if (!_validWordsTrie.Search(word))
        {
            _uiService.ShowError(Messages.WrongWordDictionary);
            return false;
        }

        return true;
    }

    private int ApplyWordScore(string word)
    {
        int points = 0;

        foreach (var letter in word.Where(l => _charScores.ContainsKey(l)))
            points += _charScores[letter];

        if (points != -1)
        {
            _game.AddScore(points);
            _game.ReelPanel.ScrollLetters(word);

            _uiService.ShowSuccess($"Great!! You have obtained {points} points!! Total points: {_game.Score}.");
            _uiService.NewLine();
        }

        return points;
    }

    private bool AskSaveGame()
    {
        _uiService.NewLine();

        var options = new string[] { "y", "n" };
        var input = _uiService.AskForInputOption(Messages.AskSave, options);
        return input == "y";
    }

    private async Task Save()
    {
        var saveResponse = await _saveGame.Execute(_game);

        _uiService.NewLine();
        if (!saveResponse.IsOk)
            _uiService.ShowError("The game couldn't be saved due to an error.");
        else
            _uiService.ShowSuccess("Game saved!!");
    }

    private void Quit(string errorMessage = "")
    {
        if (!string.IsNullOrWhiteSpace(errorMessage))
            _uiService.ShowError(errorMessage);

        _stopGame = true;

        GoodBye();
    }

    private void Welcome()
    {
        var sb = new StringBuilder();
        sb.AppendLine("**********************************************");
        sb.AppendLine($"************ {Messages.Welcome} ************");
        sb.AppendLine("**********************************************");
        sb.AppendLine();
        _uiService.ShowMessage(sb.ToString(), ConsoleColor.Cyan);

        sb.Clear();
        sb.AppendLine("Game instructions:");
        sb.AppendLine(Messages.GameInstructions);
        sb.AppendLine();
        _uiService.ShowMessage(sb.ToString());

        sb.Clear();
        sb.AppendLine("Special commands:");
        sb.AppendLine($" - '{UserKeyWords.Exit}' -> End game.");
        sb.AppendLine($" - '{UserKeyWords.Shuffle}' -> If you are not be able to find words, you can shuffle the letters with a {_shufflePenaltyPoints} points penalty.");
        sb.AppendLine();
        _uiService.ShowMessage(sb.ToString());
    }

    private void GoodBye()
    {
        _uiService.NewLine();
        var sb = new StringBuilder();
        sb.AppendLine("********************************************");
        sb.AppendLine($"************ {Messages.Thanks} ************");
        sb.AppendLine("********************************************");
        _uiService.ShowMessage(sb.ToString(), ConsoleColor.Cyan);
    }
}