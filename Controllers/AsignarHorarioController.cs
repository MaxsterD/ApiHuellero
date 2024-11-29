using ApiConsola.Interfaces.AsignarHorario;
using ApiConsola.Interfaces.ConexionHuellero;
using ApiConsola.Services.DTOs.AsignarHorario;
using Microsoft.AspNetCore.Mvc;

namespace ApiConsola.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsignarHorarioController : ControllerBase
    {
        private readonly IAsignarHorarioService _asignarHorarioService;

        public AsignarHorarioController(IAsignarHorarioService asignarHorarioService)
        {
            _asignarHorarioService = asignarHorarioService;
        }

        [HttpPost("AsignarHorario")]
        public async Task<IActionResult> AsignarHorario(AsignarHorarioDTO? datos)
        {
            var session = await _asignarHorarioService.AsignarHorario(datos);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No fue posible asignar el horario" });
        }


        [HttpGet("ListarHorariosUsuarios")]
        public async Task<IActionResult> ListarHorariosUsuarios()
        {
            var session = await _asignarHorarioService.ListarHorariosUsuarios();

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existen horarios a listar" });
        }

        [HttpPost("ActualizarHorario")]
        public async Task<IActionResult> ActualizarHorario(AsignarHorarioDTO? datos)
        {
            var session = await _asignarHorarioService.ActualizarHorario(datos);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existen horarios a actualizar" });
        }

        [HttpPost("EliminarHorario")]
        public async Task<IActionResult> EliminarHorario(AsignarHorarioDTO? datos)
        {
            var session = await _asignarHorarioService.EliminarHorario((int)datos.Id);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existen horarios a eliminar" });
        }

        
    }
}
