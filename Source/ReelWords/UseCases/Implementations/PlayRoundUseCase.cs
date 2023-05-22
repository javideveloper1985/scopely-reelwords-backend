using ReelWords.Commands;
using ReelWords.Commands.Implementations;
using ReelWords.Constants;
using ReelWords.Domain.Entities;
using ReelWords.Domain.Factories;
using ReelWords.Domain.Services;
using ReelWords.Services;
using ReelWords.UseCases.Requests;
using Scopely.Core.Enums;
using Scopely.Core.Structures;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReelWords.UseCases.Implementations;

public class PlayRoundUseCase : IPlayRoundUseCase
{
    private readonly IReelWordsUserInterfaceService _gameUiService;
    private readonly IGetLetterScoresService _scoreService;

    public PlayRoundUseCase(
        IGetLetterScoresService scoreService,
        IReelWordsUserInterfaceService gameUiService)
    {
        _scoreService = scoreService ?? throw new ArgumentNullException(nameof(scoreService));
        _gameUiService = gameUiService ?? throw new ArgumentNullException(nameof(gameUiService));
    }

    public async Task<IUserGameCommand> Execute(PlayRoundGameContext context)
    {
        try
        {
            var word = _gameUiService.InputWord();
            if (string.IsNullOrEmpty(word))
                return new InvalidWordCommand(Messages.EmptyWordLanguage);

            var specialCommand = CheckSpecialInput(word);
            if (specialCommand is not null)
                return specialCommand;

            var validationCommand = ValidateWordScore(context.Game, context.ValidationTrie, word);
            return validationCommand ?? await CalculateScore(word);
        }
        catch (Exception ex)
        {
            return new UnexpectedErrorCommand(ex);
        }
    }

    private IUserGameCommand CheckSpecialInput(string word)
    {
        if (word.Equals(UserKeyWords.Exit, StringComparison.InvariantCultureIgnoreCase))
            return new ExitGameCommand(_gameUiService.CheckIfUserWantsSaveGame());

        if (word.Equals(UserKeyWords.Shuffle, StringComparison.InvariantCultureIgnoreCase))
            return new ShuffleCommand();

        if (word.Equals(UserKeyWords.ShowWords, StringComparison.InvariantCultureIgnoreCase))
            return new ShowWordsCommand();

        if (word.Equals(UserKeyWords.Help, StringComparison.InvariantCultureIgnoreCase))
            return new HelpCommand();

        return null;
    }

    private static IUserGameCommand ValidateWordScore(Game game, Trie trie, string word)
    {
        foreach (var letter in word)
            if (!ValidCharsFactory.Get(Language.English).Contains(letter))
                return new InvalidWordCommand(Messages.WrongWordLanguage);

        if (!game.ReelPanel.CheckWord(word))
            return new InvalidWordCommand(Messages.WrongWordReel);

        if (!trie.Search(word))
            return new InvalidWordCommand(Messages.WrongWordDictionary);

        return null;
    }

    private async Task<IUserGameCommand> CalculateScore(string word)
    {
        var scores = await _scoreService.Get();

        int points = 0;
        foreach (var letter in word.Where(l => scores.ContainsKey(l)))
            points += scores[letter];

        return new WordSubmittedCommand(word, points);
    }
}