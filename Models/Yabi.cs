using Microsoft.EntityFrameworkCore;

namespace Yabi.Models
{
    public class Yabi
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string? Index { get; set; }
    }


    class YabiDb : DbContext
    {
        public YabiDb(DbContextOptions options) : base(options) { }
        public DbSet<Yabi> Yabis { get; set; } = null!;
    }
}
