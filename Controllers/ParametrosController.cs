using ApiConsola.Interfaces.AsignarLideres;
using ApiConsola.Interfaces.Parametros;
using ApiConsola.Services.DTOs.AsignarLideres;
using ApiConsola.Services.DTOs.Parametros;
using Microsoft.AspNetCore.Mvc;

namespace ApiConsola.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParametrosController : Controller
    {
        private readonly IParametrosService _parametroService;

        public ParametrosController(IParametrosService parametroService)
        {
            _parametroService = parametroService;
        }

        [HttpGet("ListarParametros")]
        public async Task<IActionResult> ListarParametros()
        {
            var session = await _parametroService.ListarParametros();

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existen parametros creados" });
        }

        [HttpPost("ActualizarParametro")]
        public async Task<IActionResult> ActualizarParametro(ParametrosDTO? datos)
        {
            var session = await _parametroService.ActualizarParametro(datos);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existe el parametro a actualizar" });
        }
    }
}
