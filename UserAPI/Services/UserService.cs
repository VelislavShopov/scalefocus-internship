using UserAPI.Models;
using UserAPI.Repositories;
using UserAPI.DTOs;
using UserAPI.Utils;
using Microsoft.AspNetCore.Http.HttpResults;

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

        //Login метод, добавен от Георги Станков
        //Добавил съм using userAPI.Utils;

        public async Task<string> LoginAsync(UserDTO request)
        {

            if (User = null)
            {

                return null;

            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
               == PasswordVerificationResult.Failed)
            {

                //Има автоматични методи за хеширане във framework-а, може да ги погледнем.

                //Не съм много сигурен, че използва файла в папката util.
                return null;

            }

            return request;

        }
    }

}
