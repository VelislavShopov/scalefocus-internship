using UserAPI.Models;
using UserAPI.Repositories;
using UserAPI.DTOs;
using Microsoft.AspNetCore.Identity;
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

        // Create метод направен от Никола Гочев
        public async Task<User> CreateUser(CreateUserDTO user)

        {

            // проверяваме дали паролата и потвържедението на парола съвпадат

            if (user.Password != user.ConfirmPassword)
            {
                throw new ArgumentException("Password and Confirm Password do not match.");
            }

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            string hashedPassword = new PasswordHasher<User>().HashPassword(newUser, user.Password);

            newUser.PasswordHash = hashedPassword;

            await _userRepository.CreateUser(newUser);
            return newUser;
        }

        public async Task DeleteUser(Guid id)
        {
            await _userRepository.DeleteUser(id);

        }

        public async Task<User?> GetUser(Guid id)
        {
            return await _userRepository.GetUser(id);
        }

        //Login метод, добавен от Георги Станков
        //Добавил съм using userAPI.Utils;


        public async Task<bool> LoginAsync(LoginUserDTO request)
        {
            var user = await _userRepository.GetUserByUsername(request.Username);

            if (user == null)
            {
                return false;
            }

            var result = new PasswordHasher<User>().VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password
            );

            if (result == PasswordVerificationResult.Success)
            {
                return true;
            }

            return false;
        }


    }
}
