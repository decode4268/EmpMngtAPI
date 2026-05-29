using Microsoft.EntityFrameworkCore;

namespace EmpMngtAPI.DataModel
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<UserTbl> Users { get; set; }
        public DbSet<RoleTbl> RoleTbls { get; set; }
        public DbSet<UserRoleMapTbl> userRoleMapTbls { get; set; }
        public DbSet<LocationTbl> locationTbls { get; set; }
        public DbSet<JobPositionTbl> jobPositionTbls  { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTbl>().ToTable("users");
            base.OnModelCreating(modelBuilder);
        }
    }
}
