using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ReelWords.Domain.Entities;
using ReelWords.Domain.Repositories;
using ReelWords.Domain.Services;
using ReelWords.Infrastructure.Dto;
using ReelWords.Infrastructure.Mappers;

namespace ReelWords.Infrastructure.Repositories;

public class GameLocalFileRepository : IGameRepository
{
    public const string FolderKey = "SavedGamesFolder";

    private readonly string _rootFolder;
    private readonly string _savedGamesFolder;
    private readonly IFileService _fileService;

    public GameLocalFileRepository(
        IConfiguration configuration,
        IFileService fileService)
    {
        _savedGamesFolder = configuration[FolderKey] ??
            throw new ArgumentException($"'{FolderKey}' cannot be null or whitespace.", nameof(configuration));
        _rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
    }

    public async Task<Game?> GetGameByUserId(string userId)
    {
        var filePath = Path.Combine(_rootFolder, _savedGamesFolder, $"{userId}.txt");

        var text = _fileService.ReadFile(filePath);

        var dto = JsonConvert.DeserializeObject<GameDto>(text);

        return await Task.FromResult(GameMapper.ToDomainEntity(dto));
    }

    public async Task<string> Create(Game game)
    {
        var filePath = Path.Combine(_rootFolder, _savedGamesFolder, $"{game.UserId}.txt");

        var dto = GameMapper.ToDto(game);
        var content = JsonConvert.SerializeObject(dto);

        _fileService.WriteFile(filePath, content);

        return await Task.FromResult(game.Id);
    }
}