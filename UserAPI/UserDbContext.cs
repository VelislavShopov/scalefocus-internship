using Microsoft.EntityFrameworkCore;
using UserAPI.Models;

namespace UserAPI
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // many to many for userroles
            modelBuilder.Entity<User>()
               .HasMany(e => e.Roles)
               .WithMany(e => e.Users);

            //seed for the roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "admin" },
                new Role { Id = 2, Name = "user" }
                );

            modelBuilder.Entity<User>()
                .HasOne(e => e.RefreshToken)
                .WithOne(e => e.User)
                .HasForeignKey<RefreshToken>(e => e.UserId);

        }

    }
}
