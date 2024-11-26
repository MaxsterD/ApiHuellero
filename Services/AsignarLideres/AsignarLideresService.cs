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

        public async Task<List<TiposIdentificacionDTO>> ListarIdentificacion()
        {
            string sql = "SELECT * FROM [Seleccion].tipos_identificacion";
            var response = await _sqlServerDbContext.Database.GetDbConnection().QueryAsync<TiposIdentificacionDTO>(sql);
            return response.ToList();
        }

        
    }
}
