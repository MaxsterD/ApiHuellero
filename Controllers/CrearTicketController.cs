using ApiConsola.Interfaces;
using ApiConsola.Services.CrearTicket;
using ApiConsola.Services.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiConsola.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrearTicketController: ControllerBase
    {
        private readonly ICrearTicketService _crearTicketService;
        public CrearTicketController(ICrearTicketService crearTicketService)
        {
            _crearTicketService = crearTicketService;
        }

        [HttpPost("CrearTicket")]
        public async Task<ActionResult> CrearTicket(BodyTicketRequest ticket)
        {
            var result = await _crearTicketService.CrearTicket(ticket);
            if(result != null && result.Success)
            {
                return Ok(result);
            }
            return BadRequest(new {Message = result?.Message ?? "Error al crear el ticket"});
        }
    }
}
