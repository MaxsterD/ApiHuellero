namespace ConsolaBlazor.Services.DTOs.AsignarEmpleados
{
    public class EmpleadosDTO
    {
        public int Id { get; set; }
        public string TipoIdentificacion { get; set; }
        public string Identificacion { get; set; }
        public string Nombre { get; set; }
    }

    public class CriterioEmpleadoDTO
    {
        public string Identificacion { get; set; }
        public string Nombre { get; set; }
    }
}
