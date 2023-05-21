using Newtonsoft.Json;

namespace ReelWords.Infrastructure.Dto;

public class GameDto
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("user")]
    public string User { get; set; }

    [JsonProperty("reelPanel")]
    public List<char[]> Reels { get; set; }

    [JsonProperty("score")]
    public int Score { get; set; }

    [JsonProperty("createdOn")]
    public DateTime CreatedOn { get; set; }
}
