using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserAPI.DTOs;
using UserAPI.Models;
namespace UserAPI.Repositories
{
    public class UserRepository : UserDbContext, IUserRepository
    {

        private readonly IConfiguration _configuration;

        public UserRepository(DbContextOptions<UserDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await Users.ToListAsync();
        }

        public async Task CreateUser(User user)
        {
            await Users.AddAsync(user);
            var role = await Roles.FirstAsync(x => x.Name == "user");
            UserRoles.Add(new UserRole()
            {
                UserId = user.Id,
                RoleId = role.Id
            });
            await SaveChangesAsync();
        }

        public async Task DeleteUser(Guid id)
        {
            var user = await Users.FindAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException();
            }

            Users.Remove(user);
            await SaveChangesAsync();

        }

        public async Task<User> GetUser(Guid id)
        {
            var user = await Users.FindAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException("No user with the given id.");
            }

            return user;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            var user = await Users.FirstOrDefaultAsync(u => u.Username == username);
            return user;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
       }

            return user;
        }

        public async Task<List<UserRole>> GetRolesForUser(User user)
        {
            var userRoles = await UserRoles.Where(x => x.UserId == user.Id).ToListAsync();

            return userRoles;
        }

        public async Task<User?> GetUserByResetToken(string token)
        {
            return await Users
                .FirstOrDefaultAsync(u => u.PasswordResetToken == token);
        }

        public async Task UpdateUser(User user)
        {
            Users.Update(user);
            await SaveChangesAsync();
        }

    }
}
