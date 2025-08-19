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
using ForgotPasswordEmailSender;
using Microsoft.Extensions.Options;

namespace UserAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        private readonly IConfiguration _configuration;

        private readonly ITokenRepository _tokenRepository;

        private readonly EmailConfiguration _emailConfiguration;

        public UserService(IUserRepository userRepository, IConfiguration configuration, ITokenRepository tokenRepository, IOptions<EmailConfiguration> emailConf)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _configuration = configuration;
            _emailConfiguration = emailConf.Value;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _userRepository.GetAllUsers();
        }


        public async Task<User?> GetUser(Guid id)
        {
            return await _userRepository.GetUser(id);
        }

        // Create метод направен от Никола Гочев
        public async Task<User> CreateUser(CreateUserDTO user)
        {
            if (user == null) 
            {
                throw new ArgumentNullException("Please provide credentials.");
            }

            if (user.Password != user.ConfirmPassword)
            {
                throw new ArgumentException("Password and Confirm Password do not match.");
            }
              
              var existingEmail = await _userRepository.GetUserByEmail(user.Email);
            if (existingEmail != null)
            {
                throw new ArgumentException("A user with this email already exists.");
            }

            var existingUsername = await _userRepository.GetUserByUsername(user.Username);
            if (existingUsername != null)
            {
                throw new ArgumentException("A user with this username already exists.");
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

        public async Task DeleteUser(Guid id, Guid loggedUserId)
        {
            if (id != loggedUserId)
            {
                throw new UnauthorizedAccessException();
            }

            await _userRepository.DeleteUser(id);

        }

        public async Task<User> LoginAsync(LoginUserDTO request)
        {
            var user = await _userRepository.GetUserByUsername(request.Username);

            if (user == null)
            {
                throw new KeyNotFoundException();
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException();
            }

            return user;
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
            await _userRepository.UpdateUser(user); 

            return "Username successfully changed.";
        }

        // Забравена парола
        public async Task ForgotPassword(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                throw new KeyNotFoundException("No user found with the given email!");
            }

            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var token = Convert.ToBase64String(tokenBytes);

            var passwordResetToken = new PasswordResetToken
            {
                UserId = user.Id,
                PasswordResetTokenValue = token,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5)
            };

            await _tokenRepository.CreatePasswordResetToken(passwordResetToken);

            using var emailSender = new EmailSender(
                _emailConfiguration.From,
                _emailConfiguration.SmtpServer,
                _emailConfiguration.Port,
                _emailConfiguration.Username,
                _emailConfiguration.Password
            );

            var callbackUrl = $"token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

            IEnumerable<string> emails = [email];

            await emailSender.SendEmail(emails, "Password Reset", callbackUrl);
        }

        // Ресет на парола
        public async Task ResetPassword(string token,string email, string newPassword)
        {
            var decodedToken = Uri.UnescapeDataString(token);
            
            var passwordResetToken = await _tokenRepository.GetPasswordResetTokenByValue(decodedToken, email);
            if (passwordResetToken == null)
            {
                throw new ArgumentException("Invalid token.");
            }

            var user = await GetUser(passwordResetToken.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException("No such user!");
            }

            var passwordHasher = new PasswordHasher<User>();
            var hashedPassword = passwordHasher.HashPassword(user, newPassword);

            user.PasswordHash = hashedPassword;

            await _tokenRepository.DeletePasswordResetToken(passwordResetToken);

            await _userRepository.UpdateUser(user);
        }
    }
}


