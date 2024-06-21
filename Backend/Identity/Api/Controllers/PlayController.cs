using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlayController : ControllerBase
    {
        [HttpGet("get-players")]
        public async Task<IActionResult> GetPlayers()
        {
            return Ok(new { message = "hello from players" });
        }

    }
}
