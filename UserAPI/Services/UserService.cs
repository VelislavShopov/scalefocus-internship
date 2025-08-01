using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserAPI.DTOs;
using UserAPI.Models;
using UserAPI.Repositories;

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

        public async Task<User> LoginAsync(LoginUserDTO request)
        {
            var user = await _userRepository.GetUserByUsername(request.Username);

            if (user == null)
            {
                return null;
            }



            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return user;
        }


    }

}
