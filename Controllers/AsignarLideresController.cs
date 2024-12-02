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

        [HttpPost("GuardarEmpleadoLider")]
        public async Task<IActionResult> Guardar(AsignarLideresDTO? datos )
        {
            var session = await _lideresService.Guardar(datos);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No fue posible listar los empleados del lider" });
        }
        
        [HttpPost("ListarEmpleadosLider")]
        public async Task<IActionResult> ListarEmpleadosLider([FromQuery] AsignarLideresDTO? datos = null)
        {
            var session = await _lideresService.ListarEmpleadosLider(datos);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No fue posible listar los empleados del lider" });
        }

        [HttpPost("EliminarEmpleadosLider")]
        public async Task<IActionResult> EliminarEmpleadosLider(AsignarLideresDTO? datos)
        {
            var session = await _lideresService.EliminarEmpleadosLider(datos);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No fue posible listar los empleados del lider" });
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
