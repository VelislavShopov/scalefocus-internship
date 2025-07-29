using UserAPI.Models;
using UserAPI.Repositories;
using UserAPI.DTOs;

namespace UserAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _userRepository.GetAllUsers();
        }

        public async Task CreateUser(UserDTO user)
        {
            // creation logic here
            
        }

        public async Task DeleteUser(int id)
        {
            await _userRepository.DeleteUser(id);
        }

        public async Task<User> GetUser(int id)
        {
            return await _userRepository.GetUser(id);
        }
    }
}
