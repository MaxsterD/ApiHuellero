
using ApiConsola.Services.DTOs;

namespace ApiConsola.Interfaces
{
    public interface ISeedService
    {
        Task<SeedResponse> FillDB();
    }
    public class SeedResponse
    {
        public int TotalCreados { get; set; }
        public List<NewUsuarioDTO> NoCreados { get; set; }
    }
}
