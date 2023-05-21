using ReelWords.Domain.ValueObjects;

namespace ReelWords.Domain.Entities;

public class Game
{
    //Session Id
    public string Id { get; private set; }

    public string UserId { get; private set; }

    public ReelPanel ReelPanel { get; private set; }

    public DateTime CreatedOn { get; private set; }

    public int Score { get; private set; } = 0;

    private Game() { }

    public static Game CreateNew(
        string userId,
        ReelPanel reelPanel)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId));
        if (reelPanel is null || reelPanel.RowCount == 0)
            throw new ArgumentNullException(nameof(reelPanel));

        return new()
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            ReelPanel = reelPanel,
            CreatedOn = DateTime.Now
        };
    }

    public static Game Create(
        string id,
        string userId,
        DateTime createdOn,
        ReelPanel reelPanel,
        int score)
            => new()
            {
                Id = id,
                UserId = userId,
                CreatedOn = createdOn,
                Score = score,
                ReelPanel = reelPanel
            };

    public void AddScore(int score)
    {
        Score += score;
        if (Score < 0)
            Score = 0;
    }
}
