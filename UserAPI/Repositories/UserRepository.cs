using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using UserAPI.Models;
namespace UserAPI.Repositories
{
    public class UserRepository : UserDbContext, IUserRepository
    {
        public UserRepository(DbContextOptions<UserDbContext> options) : base(options) { }

        public async Task<List<User>> GetAllUsers()
        {
            return await Users.ToListAsync();
        }

        public async Task CreateUser(User user)
        {
            await Users.AddAsync(user);
            await SaveChangesAsync();
        }

        public async Task DeleteUser(int id)
        {
            var user = await Users.FindAsync(id);
            Users.Remove(user);
            await SaveChangesAsync();
            
        }

        public async Task<User> GetUser(int id)
        {
            var user = await Users.FindAsync(id);

            if (user == null) 
            {
                throw new Exception();
            }

            return user;
        }
    }
}
