using ApiConsola.Services.DTOs;

namespace ApiConsola.Interfaces.CreacionUsuario
{
    public interface ICreacionUsuarioService
    {
        Task<ApiResponseDTO?> CrearUsuario(UsuarioDTO datos);
        Task<List<UsuarioDTO?>?> ListarUsuarios();
        Task<List<UsuarioDTO?>?> BuscarUsuario(UsuarioDTO? datos);
        Task<ApiResponseDTO> EliminarUsuario(int idUsuario);
        Task<ApiResponseDTO> ActualizarUsuario(UsuarioDTO? datos);
    }
}
