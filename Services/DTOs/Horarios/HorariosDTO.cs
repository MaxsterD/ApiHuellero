namespace ApiConsola.Services.DTOs.Horarios
{
    public class HorariosDTO
    {
        public int Id { get; set; }
        public int IdConcepto { get; set; }
        public string CodigoConcepto { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string HoraInicio { get; set; } = string.Empty;
        public string HoraFin { get; set; } = string.Empty;
        public List<ListaDias> DiasLaborales { get; set; } = new List<ListaDias>();
    }

    public class ListaDias
    {
        public int Dia { get; set; }
    }

    public class Conceptos
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}
