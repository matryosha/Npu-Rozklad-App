using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RozkladNpuAspNetCore.Models;

namespace RozkladNpuAspNetCore.Utils
{
    public class BaseDbContext : DbContext
    {
        public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options) 
        {
            
        }

        public DbSet<RozkladUser> Users { get; set; }
    }
}
