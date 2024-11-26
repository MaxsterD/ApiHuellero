using ApiConsola.Services.DTOs;

namespace ApiConsola.Interfaces
{
    public interface ITicketService
    {
        Task<TicktDTO?> GetTicket(int numTicket);
    }
}
