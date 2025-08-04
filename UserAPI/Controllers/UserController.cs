using EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.ComponentModel;
using System.Security.Cryptography;
using UserAPI.DTOs;
using UserAPI.Models;
using UserAPI.Repositories;
using UserAPI.Services;


namespace UserAPI.Controllers

{


    [ApiController]
    [Route("[controller]s")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly UserDbContext _context;
        private readonly IEmailSender _emailSender;


        public UserController(IUserService userService, IUserRepository userRepository, UserDbContext context, IEmailSender emailSender)
        {
            _userService = userService;
            _userRepository = userRepository;
            _context = context;
            _emailSender = emailSender;

        }


        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsersList()
        {
            var message = new Message(new string[] { "test@mailinator.com" }, "Test Email", "This is the content of our email");
            _emailSender.SendEmail(message);
            return await _userService.GetAllUsers();
        }

        [HttpPost]

        public async Task<ActionResult> CreateUser(CreateUserDTO userDTO)
        {
            await _userService.CreateUser(userDTO);
            return Created();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetUser(Guid id)
        {
            try
            {
                return Ok(await _userService.GetUser(id));
            }
            catch (Exception ex)
            {
                return NotFound();
            }

        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            try
            {
                await _userService.DeleteUser(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound();
            }

        }
        //Редактиран е Login методът с токените.
        //Логиката с refresh токените е готова, както и генерирането на токена.
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<TokenResponseDTO>> Login(LoginUserDTO request)
        {
            var result = await _userService.LoginAsync(request);
            if (result is null)
            {

                return BadRequest("Invalid username or password.");

            }


            return Ok(result);

        }


        [HttpGet]
        [Authorize]
        [Route("auth-test")]
        public IActionResult AuthenticatedOnlyEndPoint()
        {

            return Ok("you are authenticated");

        }

        [Authorize(Roles = "admin")]
        [HttpGet("admin-only")]
        public IActionResult AAdminOnlyEndPoint()
        {

            return Ok("you are an admin");

        }

        [HttpPost("refresh-token")]

        public async Task<ActionResult<TokenResponseDTO>> RefreshToken(RefreshTokenRequestDTO request)
        {

            var result = await _userService.RefreshTokenAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
            {

                return Unauthorized("Invalid refresh token");

            }

            return Ok(result);

        }



        [HttpPut("change-username")]

        public async Task<IActionResult> ChangeUsername(ChangeUsernameDTO request)
        {
            var result = await _userService.ChangeUsername(
                request.OldUsername,
                request.NewUsername,
                request.Email,
                request.Password
            );


            if (result == "Username successfully changed.")
                return Ok(new { message = result });

            return BadRequest(new { message = result });
        }

        // Забравена парола

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request data.");


            var user = await _userRepository.GetUserByEmail(request.Email!);
            if (user == null)
                return BadRequest("Invalid request.");

            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var token = Convert.ToBase64String(tokenBytes);


            user.PasswordResetToken = token;
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddMinutes(5);
            await _userRepository.UpdateUser(user);


            var baseUrl = "https://localhost:7264/reset-password";
            var callbackUrl = $"{baseUrl}?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";



            var message = new Message(
                new[] { user.Email },
                "Password Reset",
                $"Please reset your password using this link: {callbackUrl}"
            );
            await _emailSender.SendEmailAsync(message);

            return Ok("Password reset instructions have been sent to your email.");

        }

       
        // Ресет на парола

        [HttpPost("reset-password")]

        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPassword)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request data.");

            var decodedToken = Uri.UnescapeDataString(resetPassword.Token);

            var user = await _userRepository.GetUserByResetToken(decodedToken);
            if (user == null)
                return BadRequest("Invalid request.");

            var passwordHasher = new PasswordHasher<User>();
            var hashedPassword = passwordHasher.HashPassword(user, resetPassword.newPassword);


            user.PasswordHash = hashedPassword;
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpires = null;

            await _userRepository.UpdateUser(user);

            return Ok("Password reset successfully.");





        }
    }
}

