
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Yabi.Models
{


    public class FearAndGreedIndex
    {

        [JsonRequired]
        [JsonPropertyName("data")]
        public List<Data>? Data { get; set; }

        public static async Task<FearAndGreedIndex?> GetData(IHttpClientFactory clientFactory)
        {
            var client = clientFactory.CreateClient();
            var response = await client.GetAsync("https://api.alternative.me/fng/");

            if (response.IsSuccessStatusCode)
            {
             
                var json = await response.Content.ReadAsStringAsync();

                FearAndGreedIndex? fearAndGreedIndex = JsonSerializer.Deserialize<FearAndGreedIndex>(json);

                return fearAndGreedIndex;
            }
            else
            {
                return null;
            }
        }

        public static async Task<int> GetScore(IHttpClientFactory clientFactory)
        {
            var FngIndex = await FearAndGreedIndex.GetData(clientFactory);
            var success = int.TryParse(FngIndex?.Data?[0].Value, out int result);

            if (success)
            {
                return Math.Abs(result - 100);
            }

            return 0;
            
        }

    }


    public class Data
    {
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }



    



}
