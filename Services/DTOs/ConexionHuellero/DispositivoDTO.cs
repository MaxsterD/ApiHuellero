namespace ApiConsola.Services.DTOs.ConexionHuellero
{
    public class DispositivoDTO
    {
        public string? Ip { get; set; }
        public int? Puerto { get; set; }
    }

    public class RaspberryDTO
    {
        public string? Host { get; set; }
        public string? Usuario { get; set; }
        public string? Contraseña { get; set; }
    }
}
