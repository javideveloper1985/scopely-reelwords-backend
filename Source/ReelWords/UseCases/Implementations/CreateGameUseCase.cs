using ReelWords.Domain.Entities;
using ReelWords.Domain.Services;
using Scopely.Core.Result;
using System;
using System.Threading.Tasks;

namespace ReelWords.UseCases.Implementations;

public class CreateGameUseCase : ICreateGameUseCase
{
    private readonly ICreateReelPanelService _createReelService;

    public CreateGameUseCase(ICreateReelPanelService createReelService)
    {
        _createReelService = createReelService ?? throw new ArgumentNullException(nameof(createReelService));
    }

    public async Task<Result<Game>> Execute(
        int wordSize, 
        string userId)
    {
        try
        {
            var reelPanel = await _createReelService.Create(wordSize);
            reelPanel.Shuffle();
            
            var game = Game.CreateNew(userId, reelPanel);

            return Result<Game>.Ok(game);
        }
        catch (Exception ex)
        {
            return Result<Game>.Unexpected(ex);
        }
    }
}