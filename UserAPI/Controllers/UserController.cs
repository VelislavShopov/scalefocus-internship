using EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.ComponentModel;
using UserAPI.DTOs;
using UserAPI.Models;
using UserAPI.Repositories;
using UserAPI.Services;


namespace UserAPI.Controllers

{


    [ApiController]
    [Route("[controller]s")]
    public class UserController : ControllerBase
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

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO dto)
        {
            try
            {
                await _userService.ResetPassword(dto.Username, dto.NewPassword);
                return Ok("Password reset successfully.");

            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
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

            // You can decide whether to return 200 or 400 based on the result
            if (result == "Username successfully changed.")
                return Ok(new { message = result });

            return BadRequest(new { message = result });
        }



    }
}

