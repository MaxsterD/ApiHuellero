using ApiConsola.Infrastructura.Data;
using ApiConsola.Services.DTOs.AsignarHorario;
using ApiConsola.Services.DTOs.Parametros;
using Microsoft.EntityFrameworkCore;
using Dapper;
using ApiConsola.Interfaces.Parametros;
using ApiConsola.Services.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace ApiConsola.Services.Parametros
{
    public class ParametrosService : IParametrosService
    {

        private readonly ISqlServerDbContext _sqlServerDbContext;
        public ParametrosService( ISqlServerDbContext sqlServerDbContext)
        {

            _sqlServerDbContext = sqlServerDbContext;

        }

        public async Task<List<ParametrosDTO?>?> ListarParametros()
        {
            string sql = @$"select Id, Parametro as Descripcion, Value as Valor from Configuracion.Parametros";
            var response = await _sqlServerDbContext.Database.GetDbConnection().QueryAsync<ParametrosDTO?>(sql);
            return response.ToList();

        }

        public async Task<ApiResponseDTO> ActualizarParametro(ParametrosDTO datos)
        {
            try
            {
                string sql = $"UPDATE [Configuracion].Parametros SET Value = @value where Id = @id";
                var response = await _sqlServerDbContext.Database.GetDbConnection().ExecuteAsync(sql, new { id = datos?.Id, value = datos?.Valor });
                return new ApiResponseDTO { Success = response > 0, Message = "Parametro actualizado exitósamente" };
            }
            catch (Exception e)
            {
                return new ApiResponseDTO { Success = false, Message = e.Message };

            }
        }
    }
}
