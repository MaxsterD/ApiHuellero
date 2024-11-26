using ApiConsola.Services.DTOs;
using ApiConsola.Services.DTOs.AsignarLideres;

namespace ApiConsola.Interfaces.AsignarLideres
{

    public interface IAsignarLideresService
    {
        Task<List<LideresDTO?>?> Buscar(LideresDTO? datos);
        Task<List<TiposIdentificacionDTO>> ListarIdentificacion();


    }
}
