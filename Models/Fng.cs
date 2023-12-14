
using System.Text.Json.Serialization;

namespace Yabi.Models
{


    public class FearAndGreedIndex
    {

        [JsonRequired]
        [JsonPropertyName("data")]
        public List<Data>? Data { get; set; }

    }

    public class Data
    {
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }




}
