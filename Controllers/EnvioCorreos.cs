using ApiConsola.Interfaces.AsignarLideres;
using ApiConsola.Services;
using ApiConsola.Services.DTOs.AsignarLideres;
using Microsoft.AspNetCore.Mvc;

namespace ApiConsola.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvioCorreos : ControllerBase
    {

        private readonly IAsignarLideresService _lideresService;

        public EnvioCorreos(IAsignarLideresService lideresService)
        {
            _lideresService = lideresService;
        }

        [HttpPost("EnviarCorreo")]
        public async Task<IActionResult> EnviarCorreo([FromQuery] CorreoDTO? datos = null)
        {
            var session = await _lideresService.Buscar(null);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existen lideres creados" });
        }

        [HttpPost("EnviarCorreosMasivos")]
        public async Task<IActionResult> EnviarCorreoMasivo([FromQuery] CorreosDTO? datos = null)
        {
            var session = await _lideresService.Buscar(null);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existen lideres creados" });
        }
    }
}
