using ApiConsola.Services.DTOs;
using ApiConsola.Services.DTOs.Parametros;

namespace ApiConsola.Interfaces.Parametros
{
    public interface IParametrosService
    {
        Task<List<ParametrosDTO?>?> ListarParametros();
        Task<ApiResponseDTO> ActualizarParametro(ParametrosDTO datos);
    }
}
