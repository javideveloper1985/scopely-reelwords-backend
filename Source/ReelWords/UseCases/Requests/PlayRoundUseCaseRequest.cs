using ReelWords.Domain.Entities;
using Scopely.Core.Structures;
using System;

namespace ReelWords.UseCases.Requests;

public class PlayRoundUseCaseRequest
{
    public Game Game { get; set; }

    public Trie Trie { get; set; }

    public int PenaltyPoints { get; set; }

    public static PlayRoundUseCaseRequest Create(
        Game game,
        Trie validationTrie,
        int penaltyPoints)
    {
        return new PlayRoundUseCaseRequest()
        {
            Game = game ?? throw new ArgumentNullException(nameof(game)),
            Trie = validationTrie ?? throw new ArgumentNullException(nameof(validationTrie)),
            PenaltyPoints = penaltyPoints
        };
    }
}
