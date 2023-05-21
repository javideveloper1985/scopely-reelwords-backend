namespace ReelWords.Domain.Entities;

public class User
{
    public string Id { get; private set; }

    public static User Create(string id) => new() { Id = id };
}