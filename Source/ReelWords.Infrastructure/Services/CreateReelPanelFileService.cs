﻿using Microsoft.Extensions.Configuration;
using ReelWords.Domain.Services;
using ReelWords.Domain.ValueObjects;

namespace ReelWords.Infrastructure.Services;

public class CreateReelPanelFileService : ICreateReelPanelService
{
    public const string FileKey = "ReelsFile";

    private readonly string _path;

    public CreateReelPanelFileService(IConfiguration configuration)
    {
        _path = configuration[FileKey] ??
            throw new ArgumentException($"'{FileKey}' cannot be null or whitespace.", nameof(configuration));
    }

    public async Task<ReelPanel> Create(int wordSize)
    {
        var lines = File.ReadAllLines(_path);

        var panel = ReelPanel.CreateEmpty(lines.Length, wordSize);
        for(int idxRow = 0; idxRow < lines.Length; idxRow++)
        {
            var letters = lines[idxRow].Replace(" ", "").ToArray();
            panel.AddReel(idxRow, letters);
        }

        return await Task.FromResult(panel);
    }
}