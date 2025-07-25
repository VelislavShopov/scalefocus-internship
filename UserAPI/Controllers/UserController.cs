using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Models;
using UserAPI.Services;

namespace UserAPI.Controllers
{
        [ApiController]
        [Route("[controller]")]
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

        
        }
}
