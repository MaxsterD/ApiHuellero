namespace ConsolaBlazor.Services.DTOs.CreacionUsuario
{
    public class UsuarioDTO
    {
        public int? Id { get; set; }
        public string? Tipo_Identificacion { get; set; }
        public int? Identificacion { get; set; }
        public string? Nombre { get; set; }
    }

    public class CriterioUsuarioDTO
    {
        public string Identificacion { get; set; }
        public string Nombre { get; set; }
    }

    public class TiposIdentificacionDTO
    {
        public int? Id { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
    }
}
