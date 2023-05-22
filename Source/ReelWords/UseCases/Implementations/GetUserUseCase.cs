using ReelWords.Commands;
using ReelWords.Commands.Implementations;
using ReelWords.Constants;
using ReelWords.Domain.Factories;
using ReelWords.Domain.ValueObjects;
using ReelWords.Services;
using Scopely.Core.Enums;
using System;

namespace ReelWords.UseCases.Implementations;

public class GetUserUseCase : IGetUserUseCase
{
    private readonly IReelWordsUserInterfaceService _gameUiService;

    public GetUserUseCase(IReelWordsUserInterfaceService gameUiService)
    {
        _gameUiService = gameUiService ?? throw new ArgumentNullException(nameof(gameUiService));
    }

    public IUserGameCommand Execute()
    {
        try
        {
            var userName = _gameUiService.InputUserName();

            var specialCommand = CheckSpecialInput(userName);
            if (specialCommand is not null)
                return specialCommand;

            var validationCommand = ValidateUserName(userName);
            return validationCommand ?? new UserNameCommand(userName);
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

        if (word.Equals(UserKeyWords.Help, StringComparison.InvariantCultureIgnoreCase))
            return new HelpCommand();

        return null;
    }

    private static IUserGameCommand ValidateUserName(string word)
    {
        foreach (var letter in word)
        {
            if (!ValidCharsFactory.Get(Language.English).Contains(letter))
                return new WrongNameCommand(Messages.InvalidUserName);
        }
        return null;
    }
}