using ReelWords.Domain.ValueObjects;

namespace ReelWords.Domain.Services;

public interface IGetLetterScoresService
{
    Task<Dictionary<char, Letter>> GetAll();
}