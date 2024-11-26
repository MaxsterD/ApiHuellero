namespace ApiConsola.Services.DTOs
{
    public class LoginValidationDTO
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Rol { get; set; }
        public string Cliente { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }
        public int PrimerSession { get; set; }
    }
}
