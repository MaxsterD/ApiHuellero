namespace ConsolaBlazor.Services.DTOs
{
    public class DatoTipoDocumentoDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }

    public class DatoCabeceraDocumetosPeriodoDTO
    {
        public int Id { get; set; }
        public int Id_Documento { get; set; }
        public string CodTipoDocumento { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public decimal Debito { get; set; }
        public decimal Credito { get; set; }
        public string Detalle { get; set; } = string.Empty;
        public string FechaCreacion { get; set; } = string.Empty;
        public string Periodo { get; set; } = string.Empty;
    }
}
