namespace ApiConsola.Services.DTOs.Horarios
{
    public class HorariosDTO
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string HoraInicio { get; set; } = string.Empty;
        public string HoraFin { get; set; } = string.Empty;
    }
}
