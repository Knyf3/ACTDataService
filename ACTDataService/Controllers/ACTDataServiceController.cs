using ACTDataService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ACTDataService.Controllers
{
    [ApiController]
    [Route("api/ACTData")]
    public class ACTDataServiceController : ControllerBase
    {
        private readonly IUserService _userService;

        public ACTDataServiceController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetUsersList")]
        public async Task<IActionResult> GetUsersList()
        {
            //var users = await _userService.GetAllUsersAsync();

            var users = await _userService.GetAllUsersAsync();
            return Ok(users); // ASP.NET Core will automatically serialize to JSON
        }
    }
}
