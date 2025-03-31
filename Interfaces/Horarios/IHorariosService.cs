using ApiConsola.Services.DTOs.Horarios;
using ApiConsola.Services.DTOs;

namespace ApiConsola.Interfaces.Horarios
{
    public interface IHorariosService
    {
        Task<ApiResponseDTO> CrearHorario(HorariosDTO? datos);
        Task<List<HorariosDTO?>?> BuscarHorarios(HorariosDTO? datos);
        Task<List<Conceptos?>?> BuscarConceptos(Conceptos? datos);
        Task<List<HorariosDTO?>?> ListarHorarios();
        Task<ApiResponseDTO> EliminarHorario(int idHorario);
        Task<ApiResponseDTO> ActualizarHorario(HorariosDTO? datos);
    }
}
