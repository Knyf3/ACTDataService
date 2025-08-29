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

        [HttpGet("GetEventLog")]
        public async Task<IActionResult> GetEventLog(string startDate, string finishDate)
        {
            //var users = await _userService.GetAllUsersAsync();

            var eventLog = await _userService.GetEventLog(startDate, finishDate);
            return Ok(eventLog); // ASP.NET Core will automatically serialize to JSON
        }


        [HttpGet("GetUsersWithGroup")]
        public async Task<IActionResult> GetUsersWithGroup()
        {
            var users = await _userService.GetUsersWithGroup();

            
            return Ok(users); // ASP.NET Core will automatically serialize to JSON
        }
    }
}
