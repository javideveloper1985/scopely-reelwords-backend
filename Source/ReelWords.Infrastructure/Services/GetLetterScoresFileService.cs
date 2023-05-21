﻿using Microsoft.Extensions.Configuration;
using ReelWords.Domain.Services;
using ReelWords.Domain.ValueObjects;

namespace ReelWords.Infrastructure.Services;

public class GetLetterScoresFileService : IGetLetterScoresService
{
    public const string FileKey = "ScoresFile";

    private readonly string _path;

    public GetLetterScoresFileService(IConfiguration configuration)
    {
        _path = configuration[FileKey] ??
            throw new ArgumentException($"'{FileKey}' cannot be null or whitespace.", nameof(configuration));
    }

    public async Task<Dictionary<char, Letter>> GetAll()
    {
        var result = new Dictionary<char, Letter>();

        var lines = File.ReadAllLines(_path);
        foreach (var line in lines)
        {
            var score = line.Split(' ');
            var letter = score[0][0];
            var value = int.Parse(score[1]);
            result.Add(letter, new Letter(letter, value));
        }

        return await Task.FromResult(result);
    }
}