using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UserAPI.DTOs;
using UserAPI.Exceptions;
using UserAPI.Models;
using UserAPI.Repositories;
using UserAPI.Services;
using UserAPI.Helpers;

namespace UserAPI.Controllers

{
    [ApiController]
    [Route("[controller]s")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IJWTHelper _jwtHelper;

        public UserController(IUserService userService,ITokenService tokenService, IJWTHelper jWTHelper)
        {
            _jwtHelper = jWTHelper;
            _userService = userService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Retrieves the list off all the users.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult{ICollection}"/> containg a list of <see cref="User"/>/>
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<ICollection<User>>> GetUsersList()
        {
            return await _userService.GetAllUsers();
        }

        /// <summary>
        /// Creates a user.
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns>
        /// If the <see cref="User"/> is created successfully ut returns <see cref="Created"/>,
        /// otherwise if there is fault in the data -> <see cref="BadRequest"/>
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> CreateUser(CreateUserDTO userDTO)
        {
            try
            {
                await _userService.CreateUser(userDTO);
                return Created();
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentException) 
            {  
                return BadRequest(ex.Message);
            }
            catch (Exception ex) 
            {
                return StatusCode(500,ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a single user
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// If there is a user with the given id -> <see cref="User"/>,
        /// otherwise -> <see cref="NotFound"/>
        /// </returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetUser(Guid id)
        {
            try
            {
                return Ok(await _userService.GetUser(id));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a user with the given id, only the same user can do it 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [Route("{id}")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            try
            {
                var loggedUserId = _jwtHelper.GetCurrentUserId();
                await _userService.DeleteUser(id, loggedUserId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("There is no user with the given id.");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex) 
            {
                return StatusCode(500,ex.Message);
            }
        }
        
        /// <summary>
        /// The user logs in with username and password
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// If the credentials are correct -> <see cref="TokenResponseDTO"/> which contains both tokens
        /// </returns>
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<TokenResponseDTO>> Login(LoginUserDTO request)
        {

            try
            {
                var user = await _userService.LoginAsync(request);
                var response = await _tokenService.CreateAccessAndRefreshTokenResponse(user, request.Audience);
                return Ok(response);
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is UnauthorizedAccessException) 
            {
                return BadRequest("Invalid username or password!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

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
        public IActionResult AdminOnlyEndPoint()
        {

            return Ok("you are an admin");

        }

        [HttpPost("refresh-token")]

        public async Task<ActionResult<TokenResponseDTO>> RefreshToken(RefreshTokenRequestDTO request)
        {
            try
            {
                var result = await _tokenService.GetTokenResponse(request);
                return Ok(result);
            }
            catch (TokenException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request data.");

            try
            {
                await _userService.ForgotPassword(request.Email!);
                return Ok("Password reset instructions have been sent to your email.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPassword)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request data.");

            try
            {
                await _userService.ResetPassword(resetPassword.Token, resetPassword.Email, resetPassword.newPassword);
                return Ok("Password reset successfully.");
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is ArgumentException)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}

