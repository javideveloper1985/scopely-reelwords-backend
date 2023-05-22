using Newtonsoft.Json;

namespace ReelWords.Infrastructure.Dto
{
    public class WordDto
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("score")]
        public int Score { get; set; }
    }
}