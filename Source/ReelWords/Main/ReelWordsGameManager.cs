using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReelWords.Commands;
using ReelWords.Commands.Implementations;
using ReelWords.Constants;
using ReelWords.Domain.Entities;
using ReelWords.Domain.Services;
using ReelWords.Services;
using ReelWords.UseCases;
using ReelWords.UseCases.Requests;
using Scopely.Core.Structures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReelWords.Main;

public class ReelWordsGameManager : IReelWordsGameManager
{
    private readonly ILoadGameUseCase _loadGame;
    private readonly ICreateGameUseCase _createGame;
    private readonly IPlayRoundUseCase _playRoundUseCase;
    private readonly ISaveGameUseCase _saveGame;
    private readonly IGetLetterScoresService _calculateScoreService;
    private readonly IGetDictionaryService _getValidWordsService;
    private readonly IReelWordsUserInterfaceService _gameUiService;
    private readonly ILogger<ReelWordsGameManager> _logger;

    private Game _game;
    private Dictionary<char, int> _letterScores;
    private Trie _validTrie;

    private bool _stopGame = false;

    private readonly int _defaultWordSize;
    private readonly int _penaltyPoints;

    //TODO: In order to create system logs, inject ILogger
    public ReelWordsGameManager(
        ILoadGameUseCase loadGame,
        ICreateGameUseCase createGame,
        IPlayRoundUseCase playRoundUseCase,
        ISaveGameUseCase saveGame,
        IGetLetterScoresService scoreService,
        IGetDictionaryService getDictionaryService,
        IReelWordsUserInterfaceService gameUiService,
        IConfiguration config,
        ILogger<ReelWordsGameManager> logger)
    {
        _loadGame = loadGame ?? throw new ArgumentNullException(nameof(loadGame));
        _createGame = createGame ?? throw new ArgumentNullException(nameof(createGame));
        _playRoundUseCase = playRoundUseCase ?? throw new ArgumentNullException(nameof(playRoundUseCase));
        _saveGame = saveGame ?? throw new ArgumentNullException(nameof(saveGame));
        _calculateScoreService = scoreService ?? throw new ArgumentNullException(nameof(scoreService));
        _getValidWordsService = getDictionaryService ?? throw new ArgumentNullException(nameof(getDictionaryService));
        _gameUiService = gameUiService ?? throw new ArgumentNullException(nameof(gameUiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        if (config is null)
            throw new ArgumentNullException(nameof(config));
        _defaultWordSize = int.TryParse(config[ConfigKeys.WordSize], out int valueSize) ? valueSize : 7;
        _penaltyPoints = int.TryParse(config[ConfigKeys.ShufflePenalty], out int valuePenalty) ? valuePenalty : 2;
    }

    private async Task LoadGameData()
    {
        _logger.LogInformation("Loading data");
        _letterScores = await _calculateScoreService.Get();
        var validWords = await _getValidWordsService.GetByWordSize(_defaultWordSize);
        _validTrie = Trie.CreateFromListOfWords(validWords);
    }

    public async Task Start()
    {
        try
        {
            _gameUiService.ShowWelcome(_penaltyPoints);

            await LoadGameData();

            var userName = _gameUiService.InputUserName();

            var exit = CheckExitKeyWord(userName);
            if (!exit)
            {
                _game = await GetSavedGame(userName);
                _game ??= await CreateNewGame(userName);
                if (_stopGame)
                    return;

                var request = PlayRoundUseCaseRequest.Create(_game, _validTrie, _penaltyPoints);

                while (!_stopGame)
                {
                    _gameUiService.ShowLevelAndScore(request.Game.PlayedWords.Count, request.Game.Score);
                    _gameUiService.ShowReelLetters(request.Game.ReelPanel, _letterScores);

                    var command = await _playRoundUseCase.PlayRound(request);

                    _stopGame = await CommandHandler(command);
                }
            }
        }
        catch (Exception ex)
        {
            _stopGame = true;
            await _gameUiService.ShowUnexpectedError($"{Messages.UnexpectedError}: {ex.Message}");
        }

        _gameUiService.ShowGoodbye();
    }

    private async Task<bool> CommandHandler(IUserGameCommand command)
    {
        if (command is ExitGameCommand exitCommand)
        {
            if (exitCommand.SaveGame)
                await Save();

            return true;
        }
        else if (command is ShuffleCommand)
        {
            _game.ReelPanel.Shuffle();
            _game.SubtractScore(_penaltyPoints);
            _gameUiService.ShowPenalty(_penaltyPoints, _game.Score);
        }
        else if (command is ShowWordsCommand)
        {
            _gameUiService.ShowWords(_game.PlayedWords);
        }
        else if (command is InvalidWordCommand invalidWordCommand)
        {
            _gameUiService.ShowWrongInput(invalidWordCommand.Message);
        }
        else if (command is WordSubmittedCommand wordSubmittedCommand)
        {
            _game.AddScore(wordSubmittedCommand.Score, wordSubmittedCommand.Value);
            _game.ReelPanel.ScrollLetters(wordSubmittedCommand.Value);
            _gameUiService.ShowWordSubmitted(wordSubmittedCommand.Value, wordSubmittedCommand.Score, _game.Score);
        }
        else
            throw new InvalidOperationException("Unknonw command.");

        return false;
    }

    private bool CheckExitKeyWord(string word)
        => word.Equals(UserKeyWords.Exit, StringComparison.InvariantCultureIgnoreCase);

    private async Task<Game?> GetSavedGame(string userName)
    {
        var gameResponse = await _loadGame.Execute(userName);
        if (!gameResponse.IsOk)
        {
            _logger.LogCritical("Error loading saved game.");
            return null;
        }

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

    private async Task Save()
    {
        var saveResponse = await _saveGame.Execute(_game);
        _gameUiService.ShowSaveGameResponse(saveResponse.IsOk);
    }
}