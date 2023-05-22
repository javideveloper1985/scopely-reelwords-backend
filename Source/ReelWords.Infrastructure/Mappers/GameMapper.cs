using ReelWords.Domain.Entities;
using ReelWords.Domain.ValueObjects;
using ReelWords.Infrastructure.Dto;

namespace ReelWords.Infrastructure.Mappers;

public static class GameMapper
{
    public static GameDto ToDto(Game game)
    {
        if (game is null)
            throw new ArgumentNullException(nameof(game));

        var list = new List<char[]>();
        for (int i = 0; i < game.ReelPanel.RowCount; i++)
            list.Add(game.ReelPanel.GetReelByRow(i));

        return new GameDto()
        {
            Id = game.Id,
            User = game.UserId,
            Reels = list,
            CreatedOn = game.CreatedOn,
            PlayedWords = game.PlayedWords.Select(w => new WordDto()
            {
                Value = w.Value,
                Score = w.Score
            }).ToList(),
            Score = game.Score
        };
    }

    public static Game ToDomainEntity(GameDto game)
    {
        var rows = game.Reels.Count;
        var columns = game.Reels[0].Count();
        var reelPanel = ReelPanel.CreateEmpty(rows, columns);
        for (int idx = 0; idx < game.Reels.Count; idx++)
            reelPanel.AddReel(idx, game.Reels[idx]);

        return Game.Create(
            game.Id,
            game.User,
            game.CreatedOn,
            reelPanel,
            game.PlayedWords.Select(w => Word.Create(w.Value, w.Score)).ToList(),
            game.Score);
    }
}
