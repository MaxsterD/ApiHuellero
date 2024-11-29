using ApiConsola.Infrastructura.Data;
using ApiConsola.Interfaces;
using ApiConsola.Interfaces.CreacionUsuario;
using ApiConsola.Services.DTOs;
using ApiConsola.Services.DTOs.AsignarLideres;
using ApiConsola.Services.Interfaces;
using Azure;
using Dapper;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiConsola.Services.CreacionUsuario
{
    public class CreacionUsuarioService : ICreacionUsuarioService
    {
        private readonly IConfiguration _configuration;
        private readonly IEnviarHttp _enviarHttp;
        private readonly ISqlServerDbContext _sqlServerDbContext;

        public CreacionUsuarioService(IConfiguration configuration, IEnviarHttp enviarHttp, ISqlServerDbContext sqlServerDbContext)
        {
            _configuration = configuration;
            _sqlServerDbContext = sqlServerDbContext;
            _enviarHttp = enviarHttp;
        }

        public async Task<ApiResponseDTO> CrearUsuario(UsuarioDTO? datos)
        {
            string sql = "SELECT * FROM [Datos].Usuarios where Identificacion = @identificacion and Tipo_Identificacion = @tipoIdentificacion";
            var responseVerif = await _sqlServerDbContext.Database.GetDbConnection().QueryFirstOrDefaultAsync<UsuarioDTO?>(sql, new { identificacion = datos.Identificacion, tipoIdentificacion = datos.Tipo_Identificacion });

            
            if (responseVerif == null)
            {
                try { 
                    Console.WriteLine("null");
                    Console.WriteLine(responseVerif);
                    sql = "INSERT INTO [Datos].Usuarios (Nombre,Tipo_Identificacion,Identificacion) VALUES (@nombre,@tipoIdentificacion,@identificacion)";
                    var response = await _sqlServerDbContext.Database.GetDbConnection().ExecuteAsync(sql, new { identificacion = datos.Identificacion, tipoIdentificacion = datos.Tipo_Identificacion , nombre = datos.Nombre});
                    return new ApiResponseDTO() { Success = response > 0, Message = $"Usuario creado con exito!",Data = response };
                }
                catch (Exception e)
                {

                    return new ApiResponseDTO { Success = false, Message = e.Message };

                }
            }
            else
            {
                Console.WriteLine("informacion");
                Console.WriteLine(responseVerif);
                return new ApiResponseDTO() { Success = false, Message = $"Ya existe un usuario con ese numero y tipo de identificacion!" };
            }

        }

        public async Task<List<UsuarioDTO?>?> BuscarUsuario(UsuarioDTO? datos)
        {
            string sql = $"SELECT * FROM [Datos].Usuarios where (identificacion like '%' + convert(varchar(50),@identificacion) + '%' or @identificacion is null ) and (nombre like '%' + @nombre + '%' or  @nombre is null)";
            var response = await _sqlServerDbContext.Database.GetDbConnection().QueryAsync<UsuarioDTO?>(sql, new { identificacion = datos.Identificacion, nombre = datos.Nombre });
            return response.ToList();
        }

        public async Task<List<UsuarioDTO?>?> ListarUsuarios()
        {
            string sql = $"SELECT * FROM [Datos].Usuarios order by id desc";
            var response = await _sqlServerDbContext.Database.GetDbConnection().QueryAsync<UsuarioDTO?>(sql);
            return response.ToList();
            
        }

        public async Task<ApiResponseDTO> EliminarUsuario(int idUsuario)
        {
            try
            {
                string sql = $"DELETE FROM [Datos].Usuarios where Id = @id";
                var response = await _sqlServerDbContext.Database.GetDbConnection().ExecuteAsync(sql, new { id = idUsuario });
                return new ApiResponseDTO { Success = response > 0, Message = "Usuario eliminado exitósamente" };
            }
            catch (Exception e)
            {

                return new ApiResponseDTO { Success = false, Message = e.Message };

            }
        }

        public async Task<ApiResponseDTO> ActualizarUsuario(UsuarioDTO? datos)
        {
            try
            {
                string sql = $"UPDATE [Datos].Usuarios SET Nombre = @nombre, Tipo_Identificacion = @tipoIdentificacion, Identificacion = @identificacion where Id = @id";
                var response = await _sqlServerDbContext.Database.GetDbConnection().ExecuteAsync(sql, new {id = datos?.Id,tipoIdentificacion = datos?.Tipo_Identificacion ,identificacion = datos?.Identificacion, nombre = datos?.Nombre });
                return new ApiResponseDTO { Success = response > 0, Message = "Usuario actualizado exitósamente" };
            }
            catch (Exception e)
            {

                return new ApiResponseDTO { Success = false, Message = e.Message };

            }

        }

    }
}
