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

        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

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

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("RolePermissions");

                entity.HasKey(e => new { e.RoleId, e.PermissionId });

                entity.HasOne(e => e.Role)
                    .WithMany()
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(e => e.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<User>()
                .HasOne(e => e.RefreshToken)
                .WithOne(e => e.User)
                .HasForeignKey<RefreshToken>(e => e.UserId);

            modelBuilder.Entity<User>()
                .HasMany(e=> e.PasswordResetTokens)
                .WithOne(e=> e.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired();
        }

    }
}
