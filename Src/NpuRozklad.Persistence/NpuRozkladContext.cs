using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("NpuRozklad.Persistence.Tests")]

namespace NpuRozklad.Persistence
{
    public class NpuRozkladContext : DbContext
    {
        internal DbSet<RozkladUserWrapper> RozkladUserWrappers { get; set; }
        
        public NpuRozkladContext()
        { }
        
        public NpuRozkladContext(DbContextOptions<NpuRozkladContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RozkladUserWrapper>()
                .HasKey(r => r.Guid);
            modelBuilder.Entity<RozkladUserWrapper>()
                .Property(r => r.FacultyGroupsTypeIds)
                .HasConversion(
                    s => JsonConvert.SerializeObject(s),
                    s => JsonConvert.DeserializeObject<List<string>>(s));
            modelBuilder.Entity<RozkladUserWrapper>()
                .Ignore(r => r.FacultyGroups);
        }
    }
}