using ApiConsola.Infrastructura.Data;
using ApiConsola.Services.DTOs.AsignarLideres;
using ApiConsola.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Dapper;
using ApiConsola.Interfaces.AsignarLideres;
using ApiConsola.Services.DTOs;
using Azure;
using static ApiConsola.Services.UserServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiConsola.Services.AsignarLideres
{
    public class AsignarLideresService : IAsignarLideresService
    {

        private readonly IConfiguration _configuration;
        private readonly IEnviarHttp _enviarHttp;
        private readonly ISqlServerDbContext _sqlServerDbContext;

        public AsignarLideresService(IConfiguration configuration, IEnviarHttp enviarHttp, ISqlServerDbContext sqlServerDbContext)
        {
            _configuration = configuration;
            _sqlServerDbContext = sqlServerDbContext;
            _enviarHttp = enviarHttp;
        }

        public async Task<List<LideresDTO?>?> Buscar(LideresDTO? datos)
        {
            string sql = $"SELECT * FROM [Datos].lideres where (identificacion like '%' + @identificacion + '%' or @identificacion is null ) and (nombre like '%' + @nombre + '%' or  @nombre is null)";
            var response = await _sqlServerDbContext.Database.GetDbConnection().QueryAsync<LideresDTO?>(sql, new { identificacion = datos.Identificacion, nombre = datos.Nombre });
            return response.ToList(); 
        }

        public async Task<ApiResponseDTO> Guardar(AsignarLideresDTO? datos)
        {
            Console.WriteLine("datos");
            Console.WriteLine(datos.IdLider);
            Console.WriteLine(datos.IdEmpleado);
            if (datos.IdLider == datos.IdEmpleado)
            {
                return new ApiResponseDTO() { Success = false, Message = $"El lider no puede ser su mismo empleado"};

            }
            else
            {
                string sql = "SELECT * FROM [Datos].LiderEmpleados where IdLider = @idLider and IdEmpleado = @idEmpleado";
                var responseVerif = await _sqlServerDbContext.Database.GetDbConnection().QueryFirstOrDefaultAsync<UsuarioDTO?>(sql, new { idLider = datos.IdLider, idEmpleado = datos.IdEmpleado });


                if (responseVerif == null)
                {
                    try
                    {
                        sql = "INSERT INTO [Datos].LiderEmpleados (IdLider,IdEmpleado) VALUES (@idLider,@idEmpleado)";
                        var response = await _sqlServerDbContext.Database.GetDbConnection().ExecuteAsync(sql, new { idLider = datos.IdLider, idEmpleado = datos.IdEmpleado });
                        return new ApiResponseDTO() { Success = response > 0, Message = $"Usuario asignado a lider con exito!", Data = response };
                    }
                    catch (Exception e)
                    {

                        return new ApiResponseDTO { Success = false, Message = e.Message };

                    }
                }
                else
                {
                    return new ApiResponseDTO() { Success = false, Message = $"Este usuario ya se encuentra asignado a un lider!" };
                }
            }

        }

        public async Task<List<EmpleadosLideresDTO>> ListarEmpleadosLider(AsignarLideresDTO? datos)
        {
            string sql = @$"select le.Id,
                                    le.IdLider,
                                    le.IdEmpleado,
                                    (u1.Tipo_Identificacion + CONVERT(VARCHAR(MAX), u1.Identificacion) + ' - ' + u1.Nombre) as NombreLider,
                                    (u2.Tipo_Identificacion + CONVERT(VARCHAR(MAX), u2.Identificacion) + ' - ' + u2.Nombre) as NombreEmpleado 
                                    from datos.LiderEmpleados LE
                                    INNER JOIN Datos.Usuarios as u1 on u1.Id = le.IdLider
                                    INNER JOIN Datos.Usuarios as u2 on u2.Id = le.IdEmpleado
                                    Where (u1.Id = @idLider or @idLider is null)";
            var response = await _sqlServerDbContext.Database.GetDbConnection().QueryAsync<EmpleadosLideresDTO?>(sql, new { idLider = datos.IdLider});
            return response.ToList();
        }

        public async Task<ApiResponseDTO> EliminarEmpleadosLider(AsignarLideresDTO? datos)
        {
            try
            {
                var sql = "DELETE FROM [Datos].LiderEmpleados WHERE Id = @id";
                var response = await _sqlServerDbContext.Database.GetDbConnection().ExecuteAsync(sql, new { id = datos.Id });
                return new ApiResponseDTO() { Success = response > 0, Message = $"Empleado eliminado de lider con exito!", Data = response };
            }
            catch (Exception e)
            {

                return new ApiResponseDTO { Success = false, Message = e.Message };

            }
        }

        public async Task<List<TiposIdentificacionDTO>> ListarIdentificacion()
        {
            string sql = "SELECT * FROM [Seleccion].tipos_identificacion";
            var response = await _sqlServerDbContext.Database.GetDbConnection().QueryAsync<TiposIdentificacionDTO>(sql);
            return response.ToList();
        }



    }
}
