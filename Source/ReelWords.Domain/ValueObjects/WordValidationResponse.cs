namespace ReelWords.Domain.ValueObjects;

public class WordValidationResponse
{
    public bool IsValid { get; private set; }
    public string? Message { get; private set; }

    public static WordValidationResponse CreateOk()
    {
        return new WordValidationResponse()
        {
            IsValid = true,
        };
    }

    public static WordValidationResponse InvalidWord(string message)
    {
        return new WordValidationResponse()
        {
            Message = message
        };
    }
}