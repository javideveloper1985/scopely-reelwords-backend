using Microsoft.Extensions.Configuration;
using ReelWords.Constants;
using ReelWords.Domain.Entities;
using ReelWords.Domain.Factories;
using ReelWords.Domain.Services;
using ReelWords.Services;
using ReelWords.UseCases;
using Scopely.Core.Enums;
using Scopely.Core.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelWords.Main;

public class ReelWordsGameManager : IReelWordsGameManager
{
    private readonly ILoadGameUseCase _loadGame;
    private readonly ICreateGameUseCase _createGame;
    private readonly ISaveGameUseCase _saveGame;
    private readonly IGetLetterScoresService _calculateScoreService;
    private readonly IGetDictionaryService _getValidWordsService;
    private readonly IReelWordsUserInterfaceService _gameUiService;

    private Game _game;
    private Dictionary<char, int> _letterScores;
    private Trie _validWordsTrie;
    private string _validChars;

    private bool _stopGame = false;
    private readonly int _defaultWordSize;
    private readonly int _shufflePenaltyPoints;

    //TODO: Inject ILogger to create system logs
    public ReelWordsGameManager(
        ILoadGameUseCase loadGame,
        ICreateGameUseCase createGame,
        ISaveGameUseCase saveGame,
        IGetLetterScoresService scoreService,
        IGetDictionaryService getDictionaryService,
        IReelWordsUserInterfaceService gameUiService,
        IConfiguration config)
    {
        _loadGame = loadGame ?? throw new ArgumentNullException(nameof(loadGame));
        _createGame = createGame ?? throw new ArgumentNullException(nameof(createGame));
        _saveGame = saveGame ?? throw new ArgumentNullException(nameof(saveGame));
        _calculateScoreService = scoreService ?? throw new ArgumentNullException(nameof(scoreService));
        _getValidWordsService = getDictionaryService ?? throw new ArgumentNullException(nameof(getDictionaryService));
        _gameUiService = gameUiService ?? throw new ArgumentNullException(nameof(gameUiService));

        if (config is null)
            throw new ArgumentNullException(nameof(config));
        _defaultWordSize = int.TryParse(config[ConfigKeys.WordSize], out int valueSize) ? valueSize : 7;
        _shufflePenaltyPoints = int.TryParse(config[ConfigKeys.ShufflePenalty], out int valuePenalty) ? valuePenalty : 2;
    }

    private async Task LoadGameData()
    {
        _letterScores = await _calculateScoreService.Get();
        var validWords = await _getValidWordsService.GetByWordSize(_defaultWordSize);
        _validWordsTrie = Trie.CreateFromListOfWords(validWords);
        _validChars = ValidCharsFactory.Get(Language.English);
    }

    public async Task Start()
    {
        try
        {
            _gameUiService.ShowWelcome(_shufflePenaltyPoints);

            await LoadGameData();

            var userName = _gameUiService.InputUserName();

            var exit = CheckExitKeyWord(userName);
            if (!exit)
            {
                _game = await GetSavedGame(userName);
                if (_stopGame)
                    return;

                _game ??= await CreateNewGame(userName);
                if (_stopGame)
                    return;

                do
                    await StartRound();
                while (!_stopGame);
            }
        }
        catch (Exception ex)
        {
            await _gameUiService.ShowUnexpectedError($"{Messages.UnexpectedError}: {ex.Message}");

            Quit();
        }

        _gameUiService.ShowGoodbye();
    }

    private async Task StartRound()
    {
        var wordPoints = -1;

        do
        {
            if (_stopGame)
                return;

            _gameUiService.ShowLevelScore(_game.PlayedWords.Count, _game.Score);
            _gameUiService.ShowReel(_game.ReelPanel, _letterScores);

            var word = _gameUiService.InputWord();
            var exit = CheckExitKeyWord(word);
            if (exit)
            {
                if (_gameUiService.CheckIfUserWantsSaveGame())
                    await Save();

                Quit();
            }
            else
            {
                if (CheckSpecialInput(word))
                    continue;

                var isValidWord = ValidateWordScore(word);
                if (isValidWord)
                    wordPoints = ApplyWordScore(word);
            }

        } while (wordPoints == -1);
    }

    private bool CheckExitKeyWord(string word)
        => word.Equals(UserKeyWords.Exit, StringComparison.InvariantCultureIgnoreCase);

    private bool CheckSpecialInput(string word)
    {
        if (word.Equals(UserKeyWords.Shuffle, StringComparison.InvariantCultureIgnoreCase))
        {
            _game.ReelPanel.Shuffle();
            _game.SubtractScore(_shufflePenaltyPoints);
            _gameUiService.ShowPenalty(_shufflePenaltyPoints, _game.Score);
            return true;
        }

        if (word.Equals(UserKeyWords.ShowWords, StringComparison.InvariantCultureIgnoreCase))
        {
            _gameUiService.ShowWords(_game.PlayedWords);
            return true;
        }

        return false;
    }

    private async Task<Game> GetSavedGame(string userName)
    {
        var gameResponse = await _loadGame.Execute(userName);
        if (!gameResponse.IsOk)
            throw new InvalidOperationException("Error loading saved game.");

        return gameResponse.Value is not null && _gameUiService.CheckIfUserWantsLoadGame()
            ? gameResponse.Value
            : null;
    }

    private async Task<Game> CreateNewGame(string userId)
    {
        var gameResponse = await _createGame.Execute(_defaultWordSize, userId);
        if (!gameResponse.IsOk)
            throw new InvalidOperationException("Error creating new game.");

        return gameResponse.Value;
    }

    private bool ValidateWordScore(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return false;

        foreach (var letter in word)
        {
            if (!_validChars.Contains(letter))
            {
                _gameUiService.ShowWrongInput(Messages.WrongWordLanguage);
                return false;
            }
        }

        if (!_game.ReelPanel.CheckWord(word))
        {
            _gameUiService.ShowWrongInput(Messages.WrongWordReel);
            return false;
        }

        if (!_validWordsTrie.Search(word))
        {
            _gameUiService.ShowWrongInput(Messages.WrongWordDictionary);
            return false;
        }

        return true;
    }

    private int ApplyWordScore(string word)
    {
        int points = 0;

        foreach (var letter in word.Where(l => _letterScores.ContainsKey(l)))
            points += _letterScores[letter];

        if (points != -1)
        {
            _game.AddScore(points, word);
            _game.ReelPanel.ScrollLetters(word);
            _gameUiService.ShowWordSubmitted(word, points, _game.Score);
        }

        return points;
    }

    private async Task Save()
    {
        var saveResponse = await _saveGame.Execute(_game);
        _gameUiService.ShowSaveGameResponse(saveResponse.IsOk);
    }

    private void Quit()
    {
        _stopGame = true;
    }
}