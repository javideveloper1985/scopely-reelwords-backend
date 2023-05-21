using ReelWords.Constants;
using ReelWords.Domain.Entities;
using ReelWords.Services;
using Scopely.Core.Result;
using System;
using System.Threading.Tasks;

namespace ReelWords.UseCases.Implementations;

public class GetCurrentUserUseCase : IGetCurrentUserUseCase
{
    private readonly IConsoleUserInterfaceService _uiService;

    public GetCurrentUserUseCase(IConsoleUserInterfaceService uiService)
    {
        _uiService = uiService ?? throw new ArgumentNullException(nameof(uiService));
    }

    public async Task<Result<User>> Execute()
    {
        try
        {
            var userId = string.Empty;
            return await Task.Run(() =>
            {
                do
                {
                    userId = _uiService.GetInput($"Please, enter an user id ('{KeyWords.Exit}' to exit program)...");
                }
                while (!ExitLoop(userId));

                return Result<User>.Ok(User.Create(userId));
            });
        }
        catch (Exception ex)
        {
            return Result<User>.Unexpected(ex);
        }
    }

    private bool ExitLoop(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            _uiService.ShowError($"Cannot insert an empty user.");
            return false;
        }

        return true;
    }
}