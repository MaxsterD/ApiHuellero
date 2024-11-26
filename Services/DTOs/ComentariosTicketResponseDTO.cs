namespace ApiConsola.Services.DTOs
{
    public class ComentariosTicketResponseDTO
    {
        public int? totalCount { get; set; }
        public int? count { get; set; }
        public List<Comentario?> comments { get; set; }
    }

    public class Comentario
    {
        public string? text { get; set; }
        public string? createdDate { get; set; }
        public int? id { get; set; }
        public CreatedBy createdBy { get; set; }
    }

    public class CreatedBy
    {
        public string? displayName { get; set; }
    }
}
