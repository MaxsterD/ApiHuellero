using ApiConsola.Services.DTOs;

namespace ApiConsola.Interfaces
{
    public interface ITicketByClienteService
    {
        Task<List<TicketByClienteDTO>?> GetTicketByCliente(string cliente, List<string> estados);
    }
}
