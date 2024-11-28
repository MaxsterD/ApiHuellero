using ApiConsola.Services.DTOs;

namespace ApiConsola.Interfaces.CreacionUsuario
{
    public interface ICreacionUsuarioService
    {
        Task<ApiResponseDTO?> CrearUsuario(UsuarioDTO datos);
        Task<List<UsuarioDTO?>?> ListarUsuarios(UsuarioDTO? datos);
        Task<List<UsuarioDTO?>?> BuscarUsuario(UsuarioDTO? datos);
        Task<ApiResponseDTO> EliminarUsuario(int idUsuario);
    }
}
