﻿namespace ApiConsola.Services.DTOs.AsignarLideres
{
    public class AsignarLideresDTO
    {
        public int? Id { get; set; }
        public int? IdLider { get; set; }
        public int? IdEmpleado { get; set; }
    }

    public class LideresDTO
    {
        public int? Id { get; set; }
        public string? Identificacion { get; set; }
        public string? Nombre { get; set; }
    }

    public class EmpleadosLideresDTO
    {
        public int? Id { get; set; }
        public int? IdLider { get; set; }
        public int? IdEmpleado { get; set; }
        public string? NombreLider { get; set; }
        public string? NombreEmpleado { get; set; }
    }
}
