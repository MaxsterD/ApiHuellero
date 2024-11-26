using ConsolaBlazor.Services.DTOs;

namespace ConsolaBlazor.Services.Interfaces
{
    public interface IHttpService
    {
        Task<List<T>?> FetchData<T>(string url);
        Task<ApiResponseDTO> FetchDataById<T>(string url, int Id);
        Task<ApiResponseDTO> CrearFn<T>(string Path, T Parametros);
        Task<ApiResponseDTO> ActualizarFn<T>(string Path, T Parametros);
        Task<ApiResponseDTO> EliminarFn(string Path, int Id);
    }
}
