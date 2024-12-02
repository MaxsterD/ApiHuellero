using ApiConsola.Services.DTOs;
using ApiConsola.Services.DTOs.AsignarLideres;

namespace ApiConsola.Interfaces.AsignarLideres
{

    public interface IAsignarLideresService
    {
        Task<List<LideresDTO?>?> Buscar(LideresDTO? datos);
        Task<ApiResponseDTO> Guardar(AsignarLideresDTO? datos);
        Task<List<TiposIdentificacionDTO>> ListarIdentificacion();
        Task<List<EmpleadosLideresDTO>> ListarEmpleadosLider(AsignarLideresDTO? datos);
        Task<ApiResponseDTO> EliminarEmpleadosLider(AsignarLideresDTO? datos);


    }
}
