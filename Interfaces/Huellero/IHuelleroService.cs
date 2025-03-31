using ApiConsola.Services.DTOs;
using ApiConsola.Services.DTOs.ConexionHuellero;

namespace ApiConsola.Interfaces.ConexionHuellero
{
    public interface IHuelleroService
    {
        Task<ApiResponseDTO> ConectarDispositivo();
        Task<ApiResponseDTO> RecibirDatos(FiltroDatosDTO? datos);
        Task<ApiResponseDTO> ObtenerRegistrosAsistencia();
        Task<ApiResponseDTO> ObtenerRegistrosAsistenciaFiltrado(DateTime? fechaFiltrada);
        Task<ApiResponseDTO> AlimentarBase();
        Task<ApiResponseDTO> BorrarRegistro(UsuarioBaseDTO? datos);
        Task<ApiResponseDTO> ActualizarRegistro(UsuarioBaseDTO? datos);
        Task<ApiResponseDTO> CrearUsuario(string nombre, string identificacion, string password, int privilege = 0, bool enabled = true);
        Task<ApiResponseDTO> ArchivoRaspBerry(string datos);
        string EstablecerFechaHoraDispositivo(DateTime nuevaFechaHora);
        Task<string> SincronizarFechaHoraConPC();
        void IniciarSincronizacionPeriodica(int intervaloSegundos);
        Task<ApiResponseDTO> CrearUsuarioPrueba();
        Task<ApiResponseDTO> ObtenerUsuariosHuellero();


    }
}
