using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;


namespace WebAPIMedicine.Models
{
    public class ItemsContext :DbContext
    {
        public DbSet<Item> Items { get; set; } = null!;
        public ItemsContext(DbContextOptions<ItemsContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
