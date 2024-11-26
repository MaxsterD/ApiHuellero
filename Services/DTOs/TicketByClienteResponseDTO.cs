namespace ApiConsola.Services.DTOs
{
    public class TicketByClienteResponseDTO
    {
        public List<Item> workItems { get; set; } = new List<Item>();
    }
    public class Item
    {
        public string id { get; set; } = string.Empty;
        public string url { get; set; } = string.Empty;
    }
}
