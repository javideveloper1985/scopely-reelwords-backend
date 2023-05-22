using ReelWords.Domain.ValueObjects;

namespace ReelWords.Domain.Entities;

public class Game
{
    //Session Id
    public string Id { get; private set; }

    public string UserId { get; private set; }

    public ReelPanel ReelPanel { get; private set; }

    public DateTime CreatedOn { get; private set; }

    public List<Word> PlayedWords { get; private set; }

    public int Score { get; private set; }

    private Game() { }

    public static Game CreateNew(
        string userId,
        ReelPanel reelPanel)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));
        if (reelPanel is null || reelPanel.RowCount == 0)
            throw new ArgumentNullException(nameof(reelPanel));

        return new()
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            ReelPanel = reelPanel,
            CreatedOn = DateTime.Now,
            PlayedWords = new List<Word>(),
            Score = 0
        };
    }

    public static Game Create(
        string id,
        string userId,
        DateTime createdOn,
        ReelPanel reelPanel,
        List<Word> playedWords,
        int score)
            => new()
            {
                Id = id,
                UserId = userId,
                CreatedOn = createdOn,
                Score = score,
                ReelPanel = reelPanel,
                PlayedWords = playedWords
            };

    public void AddScore(int score, string word)
    {
        PlayedWords.Add(Word.Create(word, score));
        if (score > 0)
            Score += score;
    }

    public void SubtractScore(int score)
    {
        var res = Score - score;
        if (res <= 0)
            Score = 0;
        else
            Score = res;
    }

    public void Shuffle() => ReelPanel.Shuffle();
}
