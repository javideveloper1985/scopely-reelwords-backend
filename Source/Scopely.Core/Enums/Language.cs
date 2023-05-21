namespace Scopely.Core.Enums;

public class Language
{
    public readonly static Language English = new("en", "English");
    public readonly static Language Spanish = new("es", "Spanish");

    private Language(string code, string name)
    {
        Code = code;
        Name = name;
    }

    public string Code { get; }
    public string Name { get; }

    public override bool Equals(object? obj)
        => obj is Language lan && lan.Code == Code;
}
