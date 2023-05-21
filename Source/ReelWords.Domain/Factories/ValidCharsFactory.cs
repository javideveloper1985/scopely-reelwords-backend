using Scopely.Core.Enums;

namespace ReelWords.Domain.Factories;

public static class ValidCharsFactory
{
    public const string EnglishLanguage = "en";

    private static readonly Dictionary<string, string> _chars = new()
    {
        [Language.English.Code] = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
    };

    public static string Get(Language language)
    {
        if (language is not null && _chars.ContainsKey(language.Code))
            return _chars[language.Code];

        return string.Empty;
    }
}