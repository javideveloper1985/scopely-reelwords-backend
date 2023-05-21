using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ReelWords.Domain.Entities;
using ReelWords.Domain.Repositories;
using ReelWords.Infrastructure.Dto;
using ReelWords.Infrastructure.Mappers;

namespace ReelWords.Infrastructure.Repositories;

public class GameLocalFileRepository : IGameRepository
{
    public const string FileKey = "SavedGamesPath";

    private readonly string _rootFolder;
    private readonly string _path;

    public GameLocalFileRepository(IConfiguration configuration)
    {
        _path = configuration[FileKey] ??
            throw new ArgumentException($"'{FileKey}' cannot be null or whitespace.", nameof(configuration));
        _rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }

    public async Task<Game?> GetGameByUserId(string userId)
    {
        var path = $"{_rootFolder}/{_path}/{userId}.txt";
        if (!File.Exists(path))
            return null;

        var text = File.ReadAllText(path);
        var dto = JsonConvert.DeserializeObject<GameDto>(text);

        return await Task.FromResult(GameMapper.ToDomainEntity(dto));
    }

    public async Task<string> Create(Game game)
    {
        var mainPath = Path.Combine(_rootFolder, _path);
        var fileName = $"{game.UserId}.txt";
        var dto = GameMapper.ToDto(game);
        var content = JsonConvert.SerializeObject(dto);

        WriteFile(mainPath, fileName, content);

        return await Task.FromResult(game.Id);
    }

    public async Task<string> Update(Game game)
        => await Create(game);

    protected virtual bool WriteFile(string mainPath, string fileName, string content)
    {
        var path = Path.Combine(mainPath, fileName);
        Directory.CreateDirectory(mainPath);

        File.WriteAllText(path, content);

        return true;
    }
}