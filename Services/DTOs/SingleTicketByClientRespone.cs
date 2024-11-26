using Newtonsoft.Json;

namespace ApiConsola.Services.DTOs
{
    public class SingleTicketByClientRespone
    {
        public int id { get; set; }
        public Fields fields { get; set; }
    }

    public class Fields
    {
        [JsonProperty("System.State")]
        public string SState { get; set; }

        [JsonProperty("System.Title")]
        public string STitle { get; set; }
        
        [JsonProperty("System.CreatedDate")]
        public string SCreatedDate { get; set; }
        
        [JsonProperty("Microsoft.VSTS.Common.ClosedDate")]
        public string? ClosedDate { get; set; }
    }
}
