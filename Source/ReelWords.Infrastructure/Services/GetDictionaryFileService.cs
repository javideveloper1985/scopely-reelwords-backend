using Microsoft.Extensions.Configuration;
using ReelWords.Domain.Services;

namespace ReelWords.Infrastructure.Services;

public class GetDictionaryFileService : IGetDictionaryService
{
    public const string FileKey = "WordsFile";

    private readonly string _path;

    public GetDictionaryFileService(IConfiguration configuration)
    {
        _path = configuration[FileKey] ??
            throw new ArgumentException($"'{FileKey}' cannot be null or whitespace.", nameof(configuration));
    }

    public async Task<List<string>> GetByWordSize(int maxWordSize)
    {
        var result = File.ReadAllLines(_path)
            .Where(word => word.Length <= maxWordSize)
            .ToList();
        return await Task.FromResult(result);
    }
}