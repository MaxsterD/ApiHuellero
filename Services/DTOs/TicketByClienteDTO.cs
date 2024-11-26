namespace ApiConsola.Services.DTOs
{
    public class TicketByClienteDTO
    {
        public int NroTicket { get; set; }
        public string Titulo { get; set; }
        public string Estado { get; set; }
        public string FechaCreacion { get; set; }
        public string? FechaCierre { get; set; }
    }
}
