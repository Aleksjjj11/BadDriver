using Microsoft.EntityFrameworkCore;

namespace BadDriver.RestApi.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=94.228.124.165;Port=5432;Database=badDriversDb;Username=taske;Password=");
        }
    }
}