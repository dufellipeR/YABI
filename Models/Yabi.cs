using Microsoft.EntityFrameworkCore;

namespace Yabi.Models
{
    public class YabiIndex
    {


        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int? Index { get; set; }

        public static async Task<YabiIndex> Build (IHttpClientFactory clientFactory)
        {
            // Yabi max score
            const int YabiMax = 21;

            YabiIndex? Yabi = null;

            var FngIndex = await FearAndGreedIndex.GetScore(clientFactory);

            var MayerIndex = await Mayer.GetScore();

            List<int> indexes = [FngIndex, MayerIndex];

            // Max sum  of indexes score
            var Max = indexes.Count * 99;

            var Sum = indexes.Sum();

            DateTime localDate = DateTime.Now;

            Yabi = new YabiIndex
            {
                DateTime = localDate,
                Index = YabiMax * Sum / Max
            };

            return Yabi;
        }
    }


    class YabiDb : DbContext
    {
        public YabiDb(DbContextOptions options) : base(options) { }
        public DbSet<YabiIndex> Yabis { get; set; } = null!;
    }
}
