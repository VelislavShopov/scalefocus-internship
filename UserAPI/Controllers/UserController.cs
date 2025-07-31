using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.ComponentModel;
using UserAPI.DTOs;
using UserAPI.Models;
using UserAPI.Services;


namespace UserAPI.Controllers
{
    [ApiController]
    [Route("[controller]s")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public UserController(IUserService userService,ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }


        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsersList()
        {
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
            var user = await _userService.LoginAsync(request);

            if (user == null)
            {

                return BadRequest("Invalid username or password.");

            }

            var response = await _tokenService.CreatetokenResponse(user);


            return Ok(response);

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
            catch (Exception ex) 
            {
                return BadRequest("Invalid refresh token");
            }
            

        }

    }
}
