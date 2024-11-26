namespace ConsolaBlazor.Services.DTOs
{
    public class UsuarioResponseDTO
    {
        public UserData Data { get; set; }
    }

    public class UserData
    {
        public string Nombre { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
    }
}
