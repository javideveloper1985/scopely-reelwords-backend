using ReelWords.Domain.Entities;
using ReelWords.Domain.Repositories;
using Scopely.Core.Result;
using System;
using System.Threading.Tasks;

namespace ReelWords.UseCases.Implementations;

public class LoadGameUseCase : ILoadGameUseCase
{
    private readonly IGameRepository _gameRepository;

    public LoadGameUseCase(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
    }

    public async Task<Result<Game>> Execute(string userId)
    {
        try
        {
            var game = await _gameRepository.GetGameByUserId(userId);
            return Result<Game>.Ok(game);
        }
        catch (Exception ex)
        {
            return Result<Game>.Unexpected(ex);
        }
    }
}