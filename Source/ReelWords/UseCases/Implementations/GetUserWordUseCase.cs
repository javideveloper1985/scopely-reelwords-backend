using ReelWords.Constants;
using ReelWords.Services;
using Scopely.Core.Result;
using System;
using System.Threading.Tasks;

namespace ReelWords.UseCases.Implementations;

//TODO: Pensar como unir esta y la de getcurrentuser con alguna clase base
public class GetUserWordUseCase : IGetUserWordUseCase
{
    private readonly IConsoleUserInterfaceService _uiService;

    public GetUserWordUseCase(IConsoleUserInterfaceService uiService)
    {
        _uiService = uiService ?? throw new ArgumentNullException(nameof(uiService));
    }

    public async Task<Result<string>> Execute()
    {
        try
        {
            var word = string.Empty;
            return await Task.Run(() =>
            {
                do
                {
                    var message = $"Enter a word using the max number of letters above...";
                    word = _uiService.GetInput(message);
                }
                while (!ExitLoop(word));

                return Result<string>.Ok(word);
            });
        }
        catch (Exception ex)
        {
            return Result<string>.Unexpected(ex);
        }
    }

    private bool ExitLoop(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            _uiService.ShowError($"Cannot insert an empty word.");
            return false;
        }

        return true;
    }
}