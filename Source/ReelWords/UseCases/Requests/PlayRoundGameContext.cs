using ReelWords.Domain.Entities;
using Scopely.Core.Structures;
using System;

namespace ReelWords.UseCases.Requests;

public class PlayRoundGameContext
{
    public Game Game { get; set; }

    public Trie ValidationTrie { get; set; }

    public int PenaltyPoints { get; set; }

    public static PlayRoundGameContext Create(
        Game game,
        Trie validationTrie,
        int penaltyPoints)
    {
        return new PlayRoundGameContext()
        {
            Game = game ?? throw new ArgumentNullException(nameof(game)),
            ValidationTrie = validationTrie ?? throw new ArgumentNullException(nameof(validationTrie)),
            PenaltyPoints = penaltyPoints
        };
    }
}
