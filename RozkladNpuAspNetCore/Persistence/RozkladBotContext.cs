using Microsoft.EntityFrameworkCore;
using RozkladNpuAspNetCore.Entities;

namespace RozkladNpuAspNetCore.Persistence
{
    public class RozkladBotContext : DbContext
    {
        public RozkladBotContext(DbContextOptions<RozkladBotContext> options) : base(options) 
        {
            
        }

        public DbSet<RozkladUser> Users { get; set; }
    }
}
