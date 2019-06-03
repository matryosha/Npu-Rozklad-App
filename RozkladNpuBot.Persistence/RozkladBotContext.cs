using Microsoft.EntityFrameworkCore;
using NpuTimetableParser;
using RozkladNpuBot.Domain.Entities;

namespace RozkladNpuBot.Persistence
{
    public class RozkladBotContext : DbContext
    {
        public RozkladBotContext(DbContextOptions<RozkladBotContext> options) : base(options) 
        {
            
        }

        public DbSet<RozkladUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>()
                .HasKey(g => g.ExternalId);
            modelBuilder.Entity<RozkladUser>()
                .HasKey(u => u.Guid);
            modelBuilder.Entity<RozkladUser>()
                .Ignore(u => u.Groups);

            base.OnModelCreating(modelBuilder);
        }
    }
}
