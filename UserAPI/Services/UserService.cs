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
using UserAPI.Repositories;

namespace UserAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        private readonly IConfiguration _configuration;

        private readonly UserDbContext _context;

        public UserService(IUserRepository userRepository, IConfiguration configuration, UserDbContext context)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _context = context;
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


        public async Task<TokenResponseDTO> LoginAsync(LoginUserDTO request)
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

            return await _userRepository.CreatetokenResponse(user);
        }

        public async Task<TokenResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO request)
        {

            return await _userRepository.RefreshTokenAsync(request);

        }

        // възобновяване на парола
        public async Task ResetPassword(string username, string newPassword)
        {
            var user = await _userRepository.GetUserByUsername(username);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var hashedPassword = new PasswordHasher<User>().HashPassword(user, newPassword);
            user.PasswordHash = hashedPassword;

            await _context.SaveChangesAsync();
        }

        public async Task<string> ChangeUsername(string oldUsername, string newUsername, string email, string password)
        {
            var user = await _userRepository.GetUserByUsername(oldUsername);
            if (user == null) return "User with the old username not found.";

            if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
                return "Email does not match our records.";

            var passwordVerificationResult = new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
                return "Incorrect password.";

            var existingUser = await _userRepository.GetUserByUsername(newUsername);
            if (existingUser != null)
                return "New username is already taken.";

            user.Username = newUsername;
            _context.Users.Update(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return "An error occurred while saving changes.";
            }

            return "Username successfully changed.";
        }
    }
}


