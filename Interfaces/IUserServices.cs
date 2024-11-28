using ApiConsola.Services.DTOs;

namespace ApiConsola.Interfaces
{
    public interface IUserServices
    {
        Task<SessionDTO?> Login(LoginDTO login);
        Task<CreateResponse> RegistrarUsuario(NewUsuarioDTO login);
        Task<CreateResponse> ActualizarUsuario(UpdateData login);
        Task<CreateResponse> BuscarUsuario(string email);
    }

    public class CreateResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
    }

    public class UpdateData
    {
        public string? Nombre { get; set; }
        public string? Password { get; set; } = string.Empty;
        public string? Correo { get; set; }
        public string? Rol { get; set; }
    }
}
