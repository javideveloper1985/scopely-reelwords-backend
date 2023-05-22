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
    private readonly IGetUserUseCase _getUser;
    private readonly ILoadGameUseCase _loadGame;
    private readonly ICreateGameUseCase _createGame;
    private readonly IPlayRoundUseCase _playRoundUseCase;
    private readonly ISaveGameUseCase _saveGame;
    private readonly IGetLetterScoresService _calculateScoreService;
    private readonly IGetDictionaryService _getValidWordsService;
    private readonly IReelWordsUserInterfaceService _gameUiService;
    private readonly ILogger<ReelWordsGameManager> _logger;

    private Game _game;
    private User _user;
    private Dictionary<char, int> _letterScores;
    private Trie _validTrie;
    private bool _exit;

    private readonly int _defaultWordSize;
    private readonly int _penaltyPoints;

    //TODO: In order to create system logs, inject ILogger
    public ReelWordsGameManager(
        IGetUserUseCase getUser,
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
        _getUser = getUser ?? throw new ArgumentNullException(nameof(getUser));
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
            await GetUserName();
            if (_exit)
                return;

            _game = await GetSavedGame(_user.Id);
            _game ??= await CreateNewGame(_user.Id);

            await PlayRound();
        }
        catch (Exception ex)
        {
            await _gameUiService.ShowUnexpectedError($"{Messages.UnexpectedError}: {ex.Message}");
        }

        _gameUiService.ShowGoodbye();
    }

    private async Task PlayRound()
    {
        var exitLoop = false;
        while (!exitLoop)
        {
            _gameUiService.ShowLevelAndScore(_game.PlayedWords.Count, _game.Score);
            _gameUiService.ShowReelLetters(_game.ReelPanel, _letterScores);

            var context = PlayRoundGameContext.Create(_game, _validTrie, _penaltyPoints);
            var command = await _playRoundUseCase.Execute(context);

            exitLoop = await CommandHandler(command);
        }
    }

    private async Task<bool> GetUserName()
    {
        var exitLoop = false;
        while (!exitLoop && _user is null)
        {
            var commandGetUser = _getUser.Execute();
            exitLoop = await CommandHandler(commandGetUser);
        }
        return exitLoop;
    }

    private async Task<bool> CommandHandler(IUserGameCommand command)
    {
        var stopProcess = false;

        switch (command)
        {
            case ExitGameCommand exitCommand:
                if (exitCommand.SaveGame)
                    await Save();
                _gameUiService.ShowGoodbye();
                stopProcess = true;
                _exit = true;
                break;
            case WrongNameCommand wrongNameCommand:
                _gameUiService.ShowWrongInput(wrongNameCommand.Message);
                break;
            case UserNameCommand userCommand:
                _user = User.Create(userCommand.UserName);
                break;
            case ShuffleCommand:
                _game.ReelPanel.Shuffle();
                _game.SubtractScore(_penaltyPoints);
                _gameUiService.ShowPenalty(_penaltyPoints, _game.Score);
                break;
            case ShowWordsCommand:
                _gameUiService.ShowWords(_game.PlayedWords);
                break;
            case HelpCommand:
                _gameUiService.ShowHelp(_penaltyPoints);
                break;
            case InvalidWordCommand invalidWordCommand:
                _gameUiService.ShowWrongInput(invalidWordCommand.Message);
                break;
            case WordSubmittedCommand wordSubmittedCommand:
                _game.AddScore(wordSubmittedCommand.Score, wordSubmittedCommand.Value);
                _game.ReelPanel.ScrollLetters(wordSubmittedCommand.Value);
                _gameUiService.ShowWordSubmitted(wordSubmittedCommand.Value, wordSubmittedCommand.Score, _game.Score);
                break;
            default:
                throw new InvalidOperationException("Unknonw command.");
        }

        return stopProcess;
    }

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