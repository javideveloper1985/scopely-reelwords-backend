namespace ReelWords.Domain.Services;

public interface IGetDictionaryService
{
    Task<List<string>> GetByWordSize(int maxWordSize);
}