using ApiConsola.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;

namespace ApiConsola.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController:ControllerBase
    {
        private readonly ISeedService _seedService;

        public SeedController(ISeedService seedService)
        {
            _seedService = seedService;
        }

        [HttpGet("fill")]
        public async Task<IActionResult> FillDB()
        {
            SeedResponse response = await _seedService.FillDB();
            if(response == null)
            {
                return BadRequest("No se creó ningun usuario");
            }
            return Ok(response);
        }
    }
}
