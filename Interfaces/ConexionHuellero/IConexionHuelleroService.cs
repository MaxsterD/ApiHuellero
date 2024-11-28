using ApiConsola.Services.DTOs;
using ApiConsola.Services.DTOs.ConexionHuellero;

namespace ApiConsola.Interfaces.ConexionHuellero
{
    public interface IConexionHuelleroService
    {
        Task<ApiResponseDTO> ConectarDispositivo();
        Task<ApiResponseDTO> RecibirDatos(FiltroDatosDTO? datos);
        Task<ApiResponseDTO> ObtenerRegistrosAsistencia();
        Task<ApiResponseDTO> ObtenerRegistrosAsistenciaFiltrado(DateTime? fechaFiltrada);
        Task<ApiResponseDTO> AlimentarBase();
        Task<ApiResponseDTO> BorrarRegistro(UsuarioBaseDTO? datos);
        string EstablecerFechaHoraDispositivo(DateTime nuevaFechaHora);
        Task<string> SincronizarFechaHoraConPC();
        void IniciarSincronizacionPeriodica(int intervaloSegundos);
        

    }
}
