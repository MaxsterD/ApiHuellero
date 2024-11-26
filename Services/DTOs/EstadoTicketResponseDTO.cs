using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace ApiConsola.Services.DTOs
{
    public class EstadoTicketResponseDTO
    {
        public int? id { get; set; }
        public Field? fields { get; set; }
    }

    public class Field
    {
        [JsonProperty("System.AssignedTo")]
        public  AssignedTo? SAssignedTo { get; set; }

        [JsonProperty("System.State")]
        public string? SState { get; set; }
        [JsonProperty("System.Title")]
        public string? STitle { get; set; }
        [JsonProperty("System.Description")]
        public string? SDescription { get; set; }
    }
    public class AssignedTo
    {
        public string? displayName { get; set; }
    }
}
