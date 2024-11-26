

using ApiConsola.Services.DTOs;

namespace ApiConsola.Interfaces
{
    public interface ITicketStatusService
    {
        Task<CreateResponse> CreatEstado(string NewEstado);
        Task<List<EstadosDTO>?> GetEstados();
        Task<CreateResponse> EditarEstado(EstadosDTO estado);
    }
}
