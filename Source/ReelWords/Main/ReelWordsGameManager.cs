using ReelWords.Constants;
using ReelWords.Domain.Entities;
using ReelWords.Domain.Factories;
using ReelWords.Domain.Services;
using ReelWords.Domain.ValueObjects;
using ReelWords.Services;
using ReelWords.UseCases;
using Scopely.Core.Enums;
using Scopely.Core.Result;
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
    private Dictionary<char, Letter> _charScores;
    private Trie _validWordsTrie;
    private string _validChars;

    private bool _stopGame = false;

    //TODO: Better obtain these values from settings
    private const int _defaultWordSize = 7;
    private const int _penaltyPoints = 2;

    //TODO: Inject ILogger to create system logs
    public ReelWordsGameManager(
        ILoadGameUseCase loadGame,
        ICreateGameUseCase createGame,
        ISaveGameUseCase saveGame,
        IGetLetterScoresService scoreService,
        IGetDictionaryService getDictionaryService,
        IConsoleUserInterfaceService userInterface)
    {
        _loadGame = loadGame ?? throw new ArgumentNullException(nameof(loadGame));
        _createGame = createGame ?? throw new ArgumentNullException(nameof(createGame));
        _saveGame = saveGame ?? throw new ArgumentNullException(nameof(saveGame));

        _calculateScoreService = scoreService ?? throw new ArgumentNullException(nameof(scoreService));
        _getValidWordsService = getDictionaryService ?? throw new ArgumentNullException(nameof(getDictionaryService));

        _uiService = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
    }

    private async Task LoadGameData()
    {
        _charScores = await _calculateScoreService.GetAll();
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

            var userId = GetUserInput($"Please, enter an user id...");
            await CheckExitKeyWord(userId);
            if (_stopGame)
                return;

            _game = await GetSavedGame(userId);
            if (_stopGame)
                return;

            _game ??= await CreateNewGame(userId);
            if (_stopGame)
                return;

            do
            {
                PrintReel();

                var wordPoints = -1;
                string word;
                do
                {
                    word = GetUserInput($"Enter a word using the max number of letters above...");
                    await CheckExitKeyWord(word);
                    if (_stopGame)
                        return;

                    if (!CheckShuffleKeyWord(word))
                    {
                        var isValidWord = ValidateWordScore(word);
                        if (isValidWord)
                            ApplyWordScore(word);
                    }

                } while (wordPoints == -1);

            }
            while (!_stopGame);
        }
        catch (Exception ex)
        {
            //TODO: Log the exception when the ILogger has been injected
            Quit("We're sorry. An unexpected error has occurred. Restart the application and try again.");
        }
    }

    private async Task CheckExitKeyWord(string word)
    {
        if (word.Equals(KeyWords.Exit, StringComparison.InvariantCultureIgnoreCase))
        {
            if (AskSaveGame())
                await Save();

            Quit();
        }
    }

    private bool CheckShuffleKeyWord(string word)
    {
        if (word.Equals(KeyWords.Shuffle, StringComparison.InvariantCultureIgnoreCase))
        {
            _uiService.ShowError($"You have lost {_penaltyPoints} points.");
            _game.AddScore(_penaltyPoints);
            _game.ReelPanel.Shuffle();
            return true;
        }

        return false;
    }
    
    private async Task<Game> GetSavedGame(string userId)
    {
        var gameResponse = await _loadGame.Execute(userId);
        if (!gameResponse.IsOk)
            Quit("Error loading saved game.");

        _uiService.ShowMessage("HEY!", ConsoleColor.Yellow);
        var input = _uiService.AskForInputOption($"There is a saved game for the user {userId}. " +
            $"Would you like to continue that game?", "y", "n");

        return input == "y" ? gameResponse.Value : null;
    }

    private async Task<Game> CreateNewGame(string userId)
    {
        var gameResponse = await _createGame.Execute(_defaultWordSize, userId);
        if (!gameResponse.IsOk)
            Quit("Error creating new game.");

        _uiService.NewLine();
        _uiService.ShowTitle(" *** NEW GAME ***");
        _uiService.NewLine();

        return gameResponse.Value;
    }

    private void PrintReel()
    {
        var currentReel = _game.ReelPanel.GetCurrentReel();
        StringBuilder reelText = new();
        reelText.Append(" | ");
        foreach (var letter in currentReel)
        {
            var score = _charScores.ContainsKey(letter) ? _charScores[letter].Score : 0;
            reelText.Append($"{char.ToUpper(letter)} ({score})");
            reelText.Append(" | ");
        }

        _uiService.ShowMessage(reelText.ToString(), ConsoleColor.Yellow);
        _uiService.NewLine();
    }

    private string GetUserInput(string message)
    {
        string word;
        do
        {
            word = _uiService.GetInput(message);
        }
        while (string.IsNullOrEmpty(word));

        _uiService.NewLine();

        return word.ToLower();
    }

    private bool ValidateWordScore(string word)
    {
        if (string.IsNullOrEmpty(word))
            return false;

        if (!_game.ReelPanel.CheckWord(word))
        {
            _uiService.ShowError($"You must use the letters of the reel.");
            return false;
        }

        foreach (var letter in word)
        {
            if (!_validChars.Contains(letter))
            {
                _uiService.ShowError($"You must use letters of English language.");
                return false;
            }
        }

        if (!_validWordsTrie.Search(word))
        {
            _uiService.ShowError($"{word} does not exist in the game dictionary.");
            return false;
        }

        return true;
    }

    private void ApplyWordScore(string word)
    {
        int points = 0;

        foreach (var letter in word.Where(l => _charScores.ContainsKey(l)))
            points += _charScores[letter].Score;

        if (points != -1)
        {
            _game.AddScore(points);
            _game.ReelPanel.ScrollLetters(word);

            _uiService.ShowSuccess($"Great!! You have obtained {points} points!! ({_game.Score} in total)");
            _uiService.NewLine();
        }
    }

    private bool AskSaveGame()
    {
        _uiService.NewLine();

        var options = new string[] { "y", "n" };
        var input = _uiService.AskForInputOption("Would you like to save the game?", options);
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
        if (!string.IsNullOrEmpty(errorMessage))
            _uiService.ShowError(errorMessage);

        _stopGame = true;

        GoodBye();
    }

    private void Welcome()
    {
        _uiService.ShowTitle("**********************************************");
        _uiService.ShowTitle("************ WELCOME TO REELWORDS ************");
        _uiService.ShowTitle("**********************************************");
        _uiService.NewLine();
        _uiService.ShowSuccess("Game instructions: You must create words with the letters that appear on the screen. " +
            "The word must exist in the game dictionary. If it exists, you will receive certain points " +
            "regarding to the corresponding letter points.");
        _uiService.NewLine();
    }

    private void GoodBye()
    {
        _uiService.ShowTitle("********************************************");
        _uiService.ShowTitle("************ THANKS FOR PLAYING ************");
        _uiService.ShowTitle("********************************************");
    }
}