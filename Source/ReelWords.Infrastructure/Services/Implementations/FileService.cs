namespace ReelWords.Infrastructure.Services.Implementations;

public class FileService : IFileService
{
    public string ReadFile(string filePath)
    {
        if (!File.Exists(filePath))
            return string.Empty;

        return File.ReadAllText(filePath);
    }

    public void WriteFile(string filePath, string content)
    {
        var folder = Path.GetDirectoryName(filePath);
        Directory.CreateDirectory(folder);
        File.WriteAllText(filePath, content);
    }
}