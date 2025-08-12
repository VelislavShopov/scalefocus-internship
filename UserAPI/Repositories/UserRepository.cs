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
    public class UserRepository : IUserRepository
    {

        private readonly IConfiguration _configuration;
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task CreateUser(User user)
        {
            await _context.Users.AddAsync(user);
            var role = await _context.Roles.FirstAsync(x => x.Name == "user");
            _context.UserRoles.Add(new UserRole()
            {
                UserId = user.Id,
                RoleId = role.Id
            });
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(Guid id)
        {
            var user = await GetUser(id);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

        }

        public async Task<User> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException("No user with the given id.");
            }

            return user;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            return user;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
       }

        public async Task<List<UserRole>> GetRolesForUser(User user)
        {
            var userRoles = await _context.UserRoles.Where(x => x.UserId == user.Id).ToListAsync();

            return userRoles;
        }

        public async Task UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

    }
}
