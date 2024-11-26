namespace ConsolaBlazor.Services.DTOs.Login
{
    public class SesionDTO
    {
        public string Nombre { get; set; }
        public DateTime? Expiration { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
