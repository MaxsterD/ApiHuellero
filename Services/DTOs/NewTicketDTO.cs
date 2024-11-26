namespace ApiConsola.Services.DTOs
{
    public class NewTicketDTO
    {
        public List<ItemNewTicket> NewTicket { get; set; }
    }

    public class ItemNewTicket
    {
        public string op { get; set; } = "Add";
        public string path { get; set; }
        public string? from { get; set; } = null;
        public object value { get; set; }
    }

    public class BodyTicketRequest
    {
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public string? Cliente { get; set; }
        public string? Asignacion { get; set; } = "cnassif@atower.co";
    }

    public class TicketResponse
    {
        public string id { get; set; }
    }
}
