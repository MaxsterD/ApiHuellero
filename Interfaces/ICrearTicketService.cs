using ApiConsola.Services.DTOs;

namespace ApiConsola.Interfaces
{
    public interface ICrearTicketService
    {
        Task<ApiResponseDTO> CrearTicket(BodyTicketRequest ticket);
    }
}
