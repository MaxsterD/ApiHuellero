using ApiConsola.Interfaces;
using ApiConsola.Interfaces.AsignarLideres;
using ApiConsola.Services.DTOs;
using ApiConsola.Services.DTOs.AsignarLideres;
using Microsoft.AspNetCore.Mvc;

namespace ApiConsola.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsignarLideresController : ControllerBase
    {
        private readonly IAsignarLideresService _lideresService;

        public AsignarLideresController(IAsignarLideresService lideresService)
        {
            _lideresService = lideresService;
        }

        [HttpGet("ListarEmpleados")]
        public async Task<IActionResult> BuscarLider([FromQuery]LideresDTO? datos = null)
        {
            var session = await _lideresService.Buscar(datos);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existen lideres creados" });
        }

        [HttpGet("ListarIdentificacion")]
        public async Task<IActionResult> ListarIdentificacion()
        {
            var session = await _lideresService.ListarIdentificacion();

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existen tipos de identificacion" });
        }

        
    }
}
