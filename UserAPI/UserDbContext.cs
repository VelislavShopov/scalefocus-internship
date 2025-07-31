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
            modelBuilder.Entity<User>()
                .HasMany(e => e.Roles)
                .WithMany(e => e.Users)
                .UsingEntity<UserRole>();


            modelBuilder.Entity<User>()
                .HasOne(e => e.RefreshToken)
                .WithOne(e => e.User)
                .HasForeignKey<RefreshToken>(e => e.UserId);

        }

    }
}
