using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.ComponentModel;
using UserAPI.DTOs;
using UserAPI.Services;
using UserAPI.Models;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("[controller]s")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsersList()
        {
            return await _userService.GetAllUsers();
        }

        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody] UserDTO userDTO)
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
            catch (Exception ex) {
                return NotFound();
            }
           
        }
        
        //Task в контролера за имплементиране на login service.
        //Оставих още коментари във файловете User и UserDTO в DTOs и Models.
        //Засега не съм имплементирал токени.
        //Не мога да тествам, тъй като няма логика за създаване.

        [HttpPost("login")]
        public async Task<ActionResult<bool>> Login(LoginUserDTO request)
        {
            var result = await _userService.LoginAsync(request);
            if (!result)
            {

                return BadRequest("Invalid password.");

            }


            return Ok(result);

        }

    }
}
