using Microsoft.EntityFrameworkCore;

namespace BadDriver.RestApi.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievements> UserAchievements { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=94.228.124.165;Port=5432;Database=badDriversDB;Username=taske;Password=");
        }
    }
}