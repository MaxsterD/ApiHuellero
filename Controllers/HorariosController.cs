using ApiConsola.Interfaces.CreacionUsuario;
using ApiConsola.Interfaces.Horarios;
using ApiConsola.Services.DTOs.Horarios;
using Microsoft.AspNetCore.Mvc;

namespace ApiConsola.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HorariosController : ControllerBase
    {
        private readonly IHorariosService _horariosService;

        public HorariosController(IHorariosService horariosService)
        {
            _horariosService = horariosService;
        }

        [HttpPost("CrearHorario")]
        public async Task<IActionResult> CrearHorario(HorariosDTO datos)
        {
            var session = await _horariosService.CrearHorario(datos);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No fue posible crear el horario" });
        }

        [HttpGet("BuscarHorarios")]
        public async Task<IActionResult> BuscarHorarios([FromQuery] HorariosDTO? datos = null)
        {
            var session = await _horariosService.BuscarHorarios(datos);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existe el horario" });
        }
        
        [HttpGet("BuscarConceptos")]
        public async Task<IActionResult> BuscarConceptos([FromQuery] Conceptos? datos = null)
        {
            var session = await _horariosService.BuscarConceptos(datos);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existe el concepto" });
        }

        [HttpGet("ListarHorarios")]
        public async Task<IActionResult> ListarHorarios()
        {
            var session = await _horariosService.ListarHorarios();

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existen horarios creados" });
        }

        [HttpPost("EliminarHorario")]
        public async Task<IActionResult> EliminarHorario(HorariosDTO datos)
        {
            var session = await _horariosService.EliminarHorario((int)datos.Id);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existen el horarios a eliminar" });
        }

        [HttpPost("ActualizarHorario")]
        public async Task<IActionResult> ActualizarHorario(HorariosDTO? datos)
        {
            var session = await _horariosService.ActualizarHorario(datos);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existe el horarios a actualizar" });
        }
    }
}
