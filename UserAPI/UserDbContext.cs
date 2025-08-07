using Microsoft.EntityFrameworkCore;
using UserAPI.Models;

namespace UserAPI
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Many-to-many: User <-> Role
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles"); // Ensures EF maps to correct SQL table name

                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId);

                entity.HasOne<Role>()
                    .WithMany()
                    .HasForeignKey(e => e.RoleId);
            });


            modelBuilder.Entity<User>()
                .HasOne(e => e.RefreshToken)
                .WithOne(e => e.User)
                .HasForeignKey<RefreshToken>(e => e.UserId);

        }

    }
}
