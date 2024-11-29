using ApiConsola.Services.DTOs.AsignarHorario;
using ApiConsola.Services.DTOs;

namespace ApiConsola.Interfaces.AsignarHorario
{
    public interface IAsignarHorarioService
    {
        Task<ApiResponseDTO> AsignarHorario(AsignarHorarioDTO? datos);
        Task<List<HorariosUsuariosDTO?>?> ListarHorariosUsuarios();
        Task<ApiResponseDTO> ActualizarHorario(AsignarHorarioDTO? datos);
        Task<ApiResponseDTO> EliminarHorario(int? IdHorarioUsuario);
    }
}
