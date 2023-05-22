namespace ReelWords.Domain.Services;

public interface IGetLetterScoresService
{
    Task<Dictionary<char, int>> Get();
}