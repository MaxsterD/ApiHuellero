namespace ApiConsola.Services.DTOs.AsignarHorario
{

    public class AsignarHorarioDTO
    {
        public int? Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdHorario { get; set; }

    }

    public class HorariosUsuariosDTO
    {
        public int? Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdHorario { get; set; }
        public string? DescripcionUsuario { get; set; }
        public string? DescripcionHorario { get; set; }
        public string? HoraInicio { get; set; }
        public string? HoraFin { get; set; }

    }
}
