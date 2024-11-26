using ApiConsola.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiConsola.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ITicketByClienteService _ticketByClientService;

        public TicketController(ITicketService ticketService, ITicketByClienteService ticketByClientService)
        {
            _ticketService = ticketService;
            _ticketByClientService = ticketByClientService;
        }

        [HttpGet("GetTicket")]
        public async Task<IActionResult> GetTicket(int numTicket)
        {
            var ticket = await _ticketService.GetTicket(numTicket);

            if(ticket == null)
            {
                return BadRequest(new {Message = "No se encontró ticket"});
            }

            return Ok(ticket);

        }

        [HttpGet("GetTicketsByClient")]
        public async Task<IActionResult> GetTicketsByClient(string client, [FromQuery] List<string>? estados)
        {
            var tickets = await _ticketByClientService.GetTicketByCliente(client, estados);

            if (tickets == null)
            {
                return BadRequest(new { Message = "No se encontraron tickets" });
            }

            return Ok(tickets);
        }

        public class EstadosDTO
        {
            public List<string> estado { get; set; }
        }
    }
}
