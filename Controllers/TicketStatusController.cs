using ApiConsola.Interfaces;
using ApiConsola.Services.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiConsola.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketStatusController : ControllerBase
    {
        private readonly ITicketStatusService _ticketStatusService;
        public TicketStatusController(ITicketStatusService ticketStatusService)
        {
            _ticketStatusService = ticketStatusService;
        }
        [HttpGet("GetEstados")]
        public async Task<IActionResult> GetEstados()
        {
            var data = await _ticketStatusService.GetEstados();
            if(data == null)
            {
                return BadRequest(new { Message = "No se encotraron datos" });
            }
            return Ok(data);
        }

        [HttpPost("CrearEstado")]
        public async Task<IActionResult> CrearEstado(string NewEstado)
        {
            var data = await _ticketStatusService.CreatEstado(NewEstado);
            if (data == null || !data.Success)
            {
                return BadRequest(new { Message = data?.Message ?? "Error al crear el estado" });
            }
            return Ok(data);
        }
        [HttpPatch("ActualizarEstado")]
        public async Task<IActionResult> ActualizarEstado(EstadosDTO NewEstado)
        {
            var data = await _ticketStatusService.EditarEstado(NewEstado);
            if (data == null || !data.Success)
            {
                return BadRequest(new { Message = data?.Message ?? "Error al editar el estado" });
            }
            return Ok(data);
        }
    }
}
