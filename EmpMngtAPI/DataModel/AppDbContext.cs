using Microsoft.EntityFrameworkCore;

namespace EmpMngtAPI.DataModel
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<UserTbl> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTbl>().ToTable("users");
            base.OnModelCreating(modelBuilder);
        }
    }
}
