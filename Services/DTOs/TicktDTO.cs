namespace ApiConsola.Services.DTOs
{
    public class TicktDTO
    {
        public int NroTicket { get; set; }
        public string Consultor { get; set; }
        public string Estado { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public List<ComentariosTicket> Comentarios { get; set; }
    }
}
