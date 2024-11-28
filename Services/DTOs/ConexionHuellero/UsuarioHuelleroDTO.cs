namespace ApiConsola.Services.DTOs.ConexionHuellero
{
    public class UsuarioHuelleroDTO
    {
        public string? IdUsuario { get; set; }
        public string? Nombre { get; set; }
        public string? Fecha { get; set; }
        public string? Tipo { get; set; }
    }

    public class InfoUsuarioDTO
    {
        public string? IdUsuario { get; set; }
        public string? Nombre { get; set; }
        public string? Privilegio { get; set; }
        public bool Habilitado { get; set; }
    }

    public class UsuarioBaseDTO
    {
        public string? IdUsuario { get; set; }
        public string? IdHorario { get; set; }
        public string? Fecha { get; set; }
        public string? HoraEntrada { get; set; }
        public string? EstadoEntrada { get; set; }
        public string? IdEntrada { get; set; }
        public string? HoraSalida { get; set; }
        public string? EstadoSalida { get; set; }
        public string? IdSalida { get; set; }
        public string? Empleado { get; set; }

    }

    public class FiltroDatosDTO
    {
        public string? IdUsuario { get; set; }
        public string? FechaInicio { get; set; }
        public string? FechaFin { get; set; }
    }
}
