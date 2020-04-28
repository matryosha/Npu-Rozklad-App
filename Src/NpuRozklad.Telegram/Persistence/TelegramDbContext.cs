using Microsoft.EntityFrameworkCore;

namespace NpuRozklad.Telegram.Persistence
{
    internal class TelegramDbContext : DbContext
    {
        internal DbSet<TelegramRozkladUser> TelegramRozkladUsers { get; set; }

        public TelegramDbContext()
        {
            
        }

        public TelegramDbContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TelegramRozkladUser>()
                .HasKey(u => u.TelegramId);

            modelBuilder.Entity<TelegramRozkladUser>()
                .Ignore(u => u.FacultyGroups);
        }
    }
}