namespace ReelWords.Infrastructure.Services;

public interface IFileService
{
    string ReadFile(string filePath);
    void WriteFile(string filePath, string content);
}
