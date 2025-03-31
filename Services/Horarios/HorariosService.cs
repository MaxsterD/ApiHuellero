using ApiConsola.Infrastructura.Data;
using ApiConsola.Services.DTOs;
using ApiConsola.Services.DTOs.Horarios;
using ApiConsola.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Dapper;
using ApiConsola.Interfaces.Horarios;

namespace ApiConsola.Services.Horarios
{
    public class HorariosService : IHorariosService
    {
        private readonly IConfiguration _configuration;
        private readonly IEnviarHttp _enviarHttp;
        private readonly ISqlServerDbContext _sqlServerDbContext;

        public HorariosService(IConfiguration configuration, IEnviarHttp enviarHttp, ISqlServerDbContext sqlServerDbContext)
        {
            _configuration = configuration;
            _sqlServerDbContext = sqlServerDbContext;
            _enviarHttp = enviarHttp;
        }

        public async Task<ApiResponseDTO> CrearHorario(HorariosDTO? datos)
        {
            try
            { 
                var sql = @$"INSERT INTO [Datos].Horarios (Descripcion,HoraInicio,HoraFin, CodigoConcepto, IdConcepto) VALUES (@descripcion,@horaInicio,@horaFin,@codigoConcepto,@idConcepto) 
                                SELECT SCOPE_IDENTITY() AS IdHorario; ";
                var response = await _sqlServerDbContext.Database.GetDbConnection().QueryFirstOrDefaultAsync<int?>(sql, new { descripcion = datos.Descripcion, horaInicio = datos.HoraInicio, horaFin = datos.HoraFin,codigoConcepto = datos.CodigoConcepto, idConcepto = datos.IdConcepto });

                var idHorario = response;
                
                foreach (ListaDias dia in datos.DiasLaborales)
                {
                    try
                    {
                        string sqlD = $"INSERT INTO [Datos].HorariosDias (IdHorario, DiaSemana) VALUES (@IdHorario, @DiaLaboral);";
                        var responseD = await _sqlServerDbContext.Database.GetDbConnection().ExecuteAsync(sqlD, new { IdHorario = idHorario,DiaLaboral = dia.Dia });
                    }
                    catch(Exception e)
                    {
                        return new ApiResponseDTO { Success = false, Message = e.Message };

                    }
                    
                }
                
                if (response > 0)
                {
                    return new ApiResponseDTO() { Success = response > 0, Message = $"Horario creado con exito!", Data = response };
                }
                else
                {
                    return new ApiResponseDTO() { Success = response > 0, Message = $"Hubo un error al crear el horario!", Data = response };
                }
            }
            catch (Exception e)
            {

                return new ApiResponseDTO { Success = false, Message = e.Message };

            }
        }

        public async Task<List<HorariosDTO?>?> BuscarHorarios(HorariosDTO? datos)
        {
            string sql = $"SELECT * FROM [Datos].Horarios where (Descripcion like '%' + convert(varchar(50),@descripcion) + '%' or @descripcion is null )";
            var response = await _sqlServerDbContext.Database.GetDbConnection().QueryAsync<HorariosDTO?>(sql, new { descripcion = datos.Descripcion});

            if (response.Any())
            {
                foreach (var horario in response)
                {
                    string diasSql = @"
                    SELECT DiaSemana AS Dia 
                    FROM [Datos].HorariosDias 
                    WHERE IdHorario = @IdH;
                    ";

                    var diasHorario = (await _sqlServerDbContext.Database.GetDbConnection()
                        .QueryAsync<ListaDias>(diasSql, new { IdH = horario.Id }))
                        .ToList();

                    horario.DiasLaborales = diasHorario;
                }

                
            }

            return response.ToList();
        }

        public async Task<List<Conceptos?>?> BuscarConceptos(Conceptos? datos)
        {
            string sql = $"SELECT id,codigo, nombre as Descripcion FROM [Datos].conceptos where (nombre like '%' + convert(varchar(50),@descripcion) + '%' or @descripcion is null ) and (codigo like '%' + convert(varchar(50),@codigo) + '%' or @codigo is null )";
            var response = await _sqlServerDbContext.Database.GetDbConnection().QueryAsync<Conceptos?>(sql, new { descripcion = datos.Descripcion, codigo = datos.Codigo });
            return response.ToList();
        }

        public async Task<List<HorariosDTO?>?> ListarHorarios()

        {
            string sql = $"SELECT * FROM [Datos].Horarios order by id desc";
            var response = await _sqlServerDbContext.Database.GetDbConnection().QueryAsync<HorariosDTO?>(sql);

            if (response.Any())
            {
                foreach (var horario in response)
                {
                    string diasSql = @"
                    SELECT DiaSemana AS Dia 
                    FROM [Datos].HorariosDias 
                    WHERE IdHorario = @IdH;
                    ";

                    var diasHorario = (await _sqlServerDbContext.Database.GetDbConnection()
                        .QueryAsync<ListaDias>(diasSql, new { IdH = horario.Id }))
                        .ToList();

                    horario.DiasLaborales = diasHorario;
                }


            }

            return response.ToList();
        }

        public async Task<ApiResponseDTO> EliminarHorario(int idHorario)
        {
            try 
            { 
                string sql = $"DELETE FROM [Datos].Horarios where Id = @id";
                var response = await _sqlServerDbContext.Database.GetDbConnection().ExecuteAsync(sql, new { id = idHorario });
                if (response > 0)
                {
                    return new ApiResponseDTO() { Success = response > 0, Message = $"Horario creado con exito!", Data = response };
                }
                else
                {
                    return new ApiResponseDTO() { Success = response > 0, Message = $"Hubo un error al eliminar el horario!", Data = response };
                }
            }
            catch (Exception e)
            {

                return new ApiResponseDTO { Success = false, Message = e.Message };

            }
        }

        public async Task<ApiResponseDTO> ActualizarHorario(HorariosDTO? datos)
        {
            
            try
            {
                string sql = $"UPDATE [Datos].Horarios SET Descripcion = @descripcion, HoraInicio = @horaInicio, HoraFin = @horaFin where Id = @id";
                var response = await _sqlServerDbContext.Database.GetDbConnection().ExecuteAsync(sql, new { id = datos.Id, descripcion = datos.Descripcion, horaInicio = datos.HoraInicio, horaFin = datos.HoraFin });
                return new ApiResponseDTO { Success = response > 0, Message = "Horario actualizado exitósamente" };
            }
            catch (Exception e)
            {

                return new ApiResponseDTO { Success = false, Message = e.Message };

            }

        }
    }
}
