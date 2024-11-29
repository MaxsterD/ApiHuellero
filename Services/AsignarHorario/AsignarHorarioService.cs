using ApiConsola.Infrastructura.Data;
using ApiConsola.Interfaces.AsignarHorario;
using ApiConsola.Services.DTOs;
using ApiConsola.Services.DTOs.AsignarHorario;
using ApiConsola.Services.DTOs.ConexionHuellero;
using ApiConsola.Services.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace ApiConsola.Services.AsignarHorario
{
    public class AsignarHorarioService : IAsignarHorarioService
    {
        private readonly IConfiguration _configuration;
        private readonly IEnviarHttp _enviarHttp;
        private readonly ISqlServerDbContext _sqlServerDbContext;

        public AsignarHorarioService(IConfiguration configuration, IEnviarHttp enviarHttp, ISqlServerDbContext sqlServerDbContext)
        {
            _configuration = configuration;
            _sqlServerDbContext = sqlServerDbContext;
            _enviarHttp = enviarHttp;
        }

        public async Task<ApiResponseDTO> AsignarHorario(AsignarHorarioDTO? datos)
        {
            string sql = "SELECT * FROM [Datos].HorariosUsuarios where IdUsuario = @idUsuario and IdHorario = @idHorario";
            var responseVerif = await _sqlServerDbContext.Database.GetDbConnection().QueryFirstOrDefaultAsync<AsignarHorarioDTO?>(sql, new { idUsuario = datos?.IdUsuario, idHorario = datos?.IdHorario });


            if (responseVerif == null)
            {
                try
                {

                    sql = "INSERT INTO [Datos].HorariosUsuarios (IdUsuario,IdHorario) VALUES (@idUsuario,@idHorario)";
                    Console.WriteLine("Datos registro");
                    Console.WriteLine(datos?.IdUsuario);
                    Console.WriteLine(datos?.IdHorario);
                    var response = await _sqlServerDbContext.Database.GetDbConnection().ExecuteAsync(sql, new { idUsuario = datos?.IdUsuario, idHorario = datos?.IdHorario });
                    return new ApiResponseDTO() { Success = response > 0, Message = $"Usuario creado con exito!", Data = response };
                }
                catch (Exception e)
                {

                    return new ApiResponseDTO { Success = false, Message = e.Message };

                }
            }
            else
            {

                return new ApiResponseDTO() { Success = false, Message = $"El usuario ya posee este horario!" };
            }

        }

        public async Task<List<HorariosUsuariosDTO?>?> ListarHorariosUsuarios()
        {
            string sql = @$"SELECT
                            HU.Id as Id,
                            U.Id as IdUsuario,
                            H.Id as IdHorario,
                            (U.Tipo_Identificacion + CONVERT(VARCHAR(MAX), U.Identificacion) + ' - ' + U.Nombre) AS DescripcionUsuario,
                            H.Descripcion as DescripcionHorario,
                            H.HoraInicio as HoraInicio,
                            H.HoraFin as HoraFin
                            FROM [Datos].HorariosUsuarios HU
                            INNER JOIN [Datos].Horarios H on H.Id = HU.IdHorario
                            INNER JOIN [Datos].Usuarios U on U.Id = HU.IdUsuario
                            order by HU.id desc";
            var response = await _sqlServerDbContext.Database.GetDbConnection().QueryAsync<HorariosUsuariosDTO?>(sql);
            return response.ToList();

        }

        public async Task<ApiResponseDTO> ActualizarHorario(AsignarHorarioDTO? datos)
        {
            string sqlIdOld = $@"Select IdHorario from [Datos].HorariosUsuarios where Id = @id";
            var idHorarioViejo = await _sqlServerDbContext.Database.GetDbConnection().QueryAsync<int?>(sqlIdOld, new { id = datos?.Id});

            string sql = @$"
                            WITH CTE_Entradas AS (
                                SELECT 
                                    E.id as IdEntrada,
                                    E.IdUsuario as IdUsuario,
                                    h.Id AS IdHorario,
                                    E.Fecha,
                                    E.Hora AS HoraEntrada,
                                    h.HoraInicio,
                                    CASE 
                                        WHEN CAST(E.Hora AS DATETIME) BETWEEN DATEADD(MINUTE, -convert(bigint,p.Value), CAST(h.HoraInicio AS DATETIME)) 
                                                                          AND DATEADD(MINUTE, convert(bigint,p.Value), CAST(h.HoraInicio AS DATETIME)) 
                                        THEN 'Temprano'
                                        ELSE 'Tarde'
                                    END AS EstadoEntrada,
                                    ROW_NUMBER() OVER (
                                        PARTITION BY E.IdUsuario, E.Fecha, h.HoraInicio
                                        ORDER BY ABS(DATEDIFF(SECOND, CAST(h.HoraInicio AS DATETIME), CAST(E.Hora AS DATETIME))) DESC
                                    ) AS RowNum
                                FROM  
                                    DATOS.Entradas AS E
                                JOIN 
                                    Datos.HorariosUsuarios HU ON HU.IdUsuario = E.IdUsuario
                                JOIN 
                                    Datos.Horarios H ON H.Id = HU.idHorario
                                JOIN 
                                    Configuracion.Parametros P ON P.Id = 1
                                WHERE
                                    E.Hora BETWEEN DATEADD(MINUTE, -CONVERT(bigint, P.Value), H.HoraInicio) 
                                             AND DATEADD(MINUTE, CONVERT(bigint, P.Value), H.HoraInicio)
                                    AND (E.IdUsuario = @idUsuario or @idUsuario is null)
                                    AND ((E.Fecha BETWEEN @FechaInicio and @FechaFin) or (@FechaInicio is null and @FechaFin is null))
                            ),
                            CTE_Salidas AS (
                                SELECT 
                                    S.id as IdSalida,
                                    S.IdUsuario,
                                    h.Id AS IdHorario,
                                    S.Fecha,
                                    S.Hora AS HoraSalida,
                                    h.HoraFin,
                                    CASE 
                                        WHEN CAST(S.Hora AS DATETIME) BETWEEN DATEADD(MINUTE, -CONVERT(bigint, P.Value), CAST(h.HoraFin AS DATETIME)) 
                                                                          AND DATEADD(MINUTE, CONVERT(bigint, P.Value), CAST(h.HoraFin AS DATETIME)) 
                                        THEN 'Temprano'
                                        ELSE 'Tarde'
                                    END AS EstadoSalida,
                                    ROW_NUMBER() OVER (
                                        PARTITION BY S.IdUsuario, S.Fecha, h.HoraFin
                                        ORDER BY ABS(DATEDIFF(SECOND, CAST(h.HoraFin AS DATETIME), CAST(S.Hora AS DATETIME))) DESC
                                    ) AS RowNum
                                FROM  
                                    DATOS.Salidas S
                                JOIN 
                                    datos.HorariosUsuarios HU on HU.IdUsuario = s.IdUsuario
                                JOIN 
                                    Datos.Horarios H on H.Id = HU.idHorario
                                JOIN 
                                    Configuracion.Parametros P ON P.Id = 1 
                                WHERE
                                    S.Hora BETWEEN DATEADD(MINUTE, -CONVERT(bigint, P.Value), H.HoraFin) 
                                             AND DATEADD(MINUTE, CONVERT(bigint, P.Value), H.HoraFin)
                                    AND (S.IdUsuario = @idUsuario or @idUsuario is null)
                                    AND ((S.Fecha BETWEEN @FechaInicio and @FechaFin) or (@FechaInicio is null and @FechaFin is null))
                            )
                            SELECT 
                                ISNULL(e.IdUsuario, s.IdUsuario) AS IdUsuario,
                                ISNULL(e.IdHorario, s.IdHorario) AS IdHorario,
                                ISNULL(e.Fecha, s.Fecha) AS Fecha,
                                e.HoraEntrada,
                                e.EstadoEntrada,
                                e.IdEntrada,
                                s.HoraSalida,
                                s.EstadoSalida,
                                s.IdSalida,
                                e.HoraInicio,
                                s.HoraFin,
                                (u.Tipo_Identificacion + CONVERT(VARCHAR(MAX), u.Identificacion) + ' - ' + u.Nombre) AS Nombre
                            FROM 
                                CTE_Entradas e
                            LEFT JOIN 
                                CTE_Salidas s ON e.IdUsuario = s.IdUsuario 
                                             AND e.IdHorario = s.IdHorario 
                                             AND e.Fecha = s.Fecha
                            JOIN 
                                Datos.Usuarios u ON u.Id = ISNULL(e.IdUsuario, s.IdUsuario)
                            WHERE 
                                (e.RowNum = 1 OR e.RowNum IS NULL)
                                AND (s.RowNum = 1 OR s.RowNum IS NULL)
	                            AND  ISNULL(e.IdHorario, s.IdHorario) = @idHorario
                            ORDER BY 
                                e.IdUsuario, e.IdHorario, e.Fecha;";

            var responseVerif = await _sqlServerDbContext.Database.GetDbConnection().QueryAsync<UsuarioBaseDTO?>(sql, new { idUsuario = datos?.IdUsuario, idHorario = idHorarioViejo });
            
            if (responseVerif == null)
            {
                try
                {

                    sql = $"UPDATE [Datos].HorariosUsuarios SET IdHorario = @idHorario where Id = @id";
                    var response = await _sqlServerDbContext.Database.GetDbConnection().ExecuteAsync(sql, new { id = datos?.Id, idHorario = datos?.IdHorario });
                    return new ApiResponseDTO { Success = response > 0, Message = "Usuario actualizado exitósamente" };
                }
                catch (Exception e)
                {

                    return new ApiResponseDTO { Success = false, Message = e.Message };

                }
            }
            else
            {

                return new ApiResponseDTO() { Success = false, Message = $"No es posible actualizar el horario asignado, posee movientos con el usuario actual!" };
            }
        }

        public async Task<ApiResponseDTO> EliminarHorario(int? IdHorarioUsuario)
        {
            string sqlIdOld = $@"Select Id,IdUsuario,IdHorario from [Datos].HorariosUsuarios where Id = @id";
            var datosHorarioUsuario = await _sqlServerDbContext.Database.GetDbConnection().QueryFirstOrDefaultAsync<AsignarHorarioDTO?>(sqlIdOld, new { id = IdHorarioUsuario });

            string sql = @$"
                            WITH CTE_Entradas AS (
                                SELECT 
                                    E.id as IdEntrada,
                                    E.IdUsuario as IdUsuario,
                                    h.Id AS IdHorario,
                                    E.Fecha,
                                    E.Hora AS HoraEntrada,
                                    h.HoraInicio,
                                    CASE 
                                        WHEN CAST(E.Hora AS DATETIME) BETWEEN DATEADD(MINUTE, -convert(bigint,p.Value), CAST(h.HoraInicio AS DATETIME)) 
                                                                          AND DATEADD(MINUTE, convert(bigint,p.Value), CAST(h.HoraInicio AS DATETIME)) 
                                        THEN 'Temprano'
                                        ELSE 'Tarde'
                                    END AS EstadoEntrada,
                                    ROW_NUMBER() OVER (
                                        PARTITION BY E.IdUsuario, E.Fecha, h.HoraInicio
                                        ORDER BY ABS(DATEDIFF(SECOND, CAST(h.HoraInicio AS DATETIME), CAST(E.Hora AS DATETIME))) DESC
                                    ) AS RowNum
                                FROM  
                                    DATOS.Entradas AS E
                                JOIN 
                                    Datos.HorariosUsuarios HU ON HU.IdUsuario = E.IdUsuario
                                JOIN 
                                    Datos.Horarios H ON H.Id = HU.idHorario
                                JOIN 
                                    Configuracion.Parametros P ON P.Id = 1
                                WHERE
                                    E.Hora BETWEEN DATEADD(MINUTE, -CONVERT(bigint, P.Value), H.HoraInicio) 
                                             AND DATEADD(MINUTE, CONVERT(bigint, P.Value), H.HoraInicio)
                                    AND (E.IdUsuario = @idUsuario or @idUsuario is null)
                                    AND ((E.Fecha BETWEEN @FechaInicio and @FechaFin) or (@FechaInicio is null and @FechaFin is null))
                            ),
                            CTE_Salidas AS (
                                SELECT 
                                    S.id as IdSalida,
                                    S.IdUsuario,
                                    h.Id AS IdHorario,
                                    S.Fecha,
                                    S.Hora AS HoraSalida,
                                    h.HoraFin,
                                    CASE 
                                        WHEN CAST(S.Hora AS DATETIME) BETWEEN DATEADD(MINUTE, -CONVERT(bigint, P.Value), CAST(h.HoraFin AS DATETIME)) 
                                                                          AND DATEADD(MINUTE, CONVERT(bigint, P.Value), CAST(h.HoraFin AS DATETIME)) 
                                        THEN 'Temprano'
                                        ELSE 'Tarde'
                                    END AS EstadoSalida,
                                    ROW_NUMBER() OVER (
                                        PARTITION BY S.IdUsuario, S.Fecha, h.HoraFin
                                        ORDER BY ABS(DATEDIFF(SECOND, CAST(h.HoraFin AS DATETIME), CAST(S.Hora AS DATETIME))) DESC
                                    ) AS RowNum
                                FROM  
                                    DATOS.Salidas S
                                JOIN 
                                    datos.HorariosUsuarios HU on HU.IdUsuario = s.IdUsuario
                                JOIN 
                                    Datos.Horarios H on H.Id = HU.idHorario
                                JOIN 
                                    Configuracion.Parametros P ON P.Id = 1 
                                WHERE
                                    S.Hora BETWEEN DATEADD(MINUTE, -CONVERT(bigint, P.Value), H.HoraFin) 
                                             AND DATEADD(MINUTE, CONVERT(bigint, P.Value), H.HoraFin)
                                    AND (S.IdUsuario = @idUsuario or @idUsuario is null)
                                    AND ((S.Fecha BETWEEN @FechaInicio and @FechaFin) or (@FechaInicio is null and @FechaFin is null))
                            )
                            SELECT 
                                ISNULL(e.IdUsuario, s.IdUsuario) AS IdUsuario,
                                ISNULL(e.IdHorario, s.IdHorario) AS IdHorario,
                                ISNULL(e.Fecha, s.Fecha) AS Fecha,
                                e.HoraEntrada,
                                e.EstadoEntrada,
                                e.IdEntrada,
                                s.HoraSalida,
                                s.EstadoSalida,
                                s.IdSalida,
                                e.HoraInicio,
                                s.HoraFin,
                                (u.Tipo_Identificacion + CONVERT(VARCHAR(MAX), u.Identificacion) + ' - ' + u.Nombre) AS Nombre
                            FROM 
                                CTE_Entradas e
                            LEFT JOIN 
                                CTE_Salidas s ON e.IdUsuario = s.IdUsuario 
                                             AND e.IdHorario = s.IdHorario 
                                             AND e.Fecha = s.Fecha
                            JOIN 
                                Datos.Usuarios u ON u.Id = ISNULL(e.IdUsuario, s.IdUsuario)
                            WHERE 
                                (e.RowNum = 1 OR e.RowNum IS NULL)
                                AND (s.RowNum = 1 OR s.RowNum IS NULL)
	                            AND  ISNULL(e.IdHorario, s.IdHorario) = @idHorario
                            ORDER BY 
                                e.IdUsuario, e.IdHorario, e.Fecha;";

            var responseVerif = await _sqlServerDbContext.Database.GetDbConnection().QueryAsync<UsuarioBaseDTO?>(sql, new { idUsuario = datosHorarioUsuario?.IdUsuario, idHorario = datosHorarioUsuario?.IdHorario });

            if (responseVerif == null)
            {
                try
                {

                    sql = $"DELETE FROM [Datos].HorariosUsuarios where Id = @id";
                    var response = await _sqlServerDbContext.Database.GetDbConnection().ExecuteAsync(sql, new { id = datosHorarioUsuario?.Id});
                    return new ApiResponseDTO { Success = response > 0, Message = "Horario asignado elminado exitósamente" };
                }
                catch (Exception e)
                {

                    return new ApiResponseDTO { Success = false, Message = e.Message };

                }
            }
            else
            {

                return new ApiResponseDTO() { Success = false, Message = $"No es posible eliminar el horario asignado, posee movientos con el usuario actual!" };
            }
        }




    }
}
