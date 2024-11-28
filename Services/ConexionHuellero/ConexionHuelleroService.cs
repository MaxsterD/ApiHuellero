﻿using ApiConsola.Interfaces.ConexionHuellero;
using ApiConsola.Services.DTOs;
using ApiConsola.Services.Interfaces;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using zkemkeeper;
using ApiConsola.Services.DTOs.ConexionHuellero;
using ApiConsola.Infrastructura.Data;
using Dapper;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;


namespace ApiConsola.Services.ConexionHuellero
{
    public class ConexionHuelleroService : IConexionHuelleroService
    {
        private CZKEM _device;
        private int commPassword = 232425;
        private int machineNumber = 1;
        private CancellationTokenSource _cancellationTokenSource;

        private readonly IInfoDevice _infoDevice;
        private readonly ISqlServerDbContext _sqlServerDbContext;

        public ConexionHuelleroService(IInfoDevice infoDevice , ISqlServerDbContext sqlServerDbContext)
        {
            _infoDevice = infoDevice;
            _device = new zkemkeeper.CZKEM();
            _sqlServerDbContext = sqlServerDbContext;
            
        }

        public async Task<ApiResponseDTO> ConectarDispositivo()
        {
            string sql = "SELECT * FROM [Configuracion].dispositivo ";
            var configuracionHuellero = await _sqlServerDbContext.Database.GetDbConnection().QueryFirstOrDefaultAsync<DispositivoDTO?>(sql);

            bool passwordSet = _device.SetCommPassword(commPassword);
            if (!passwordSet)
            {
                return new ApiResponseDTO() { Success = false, Message = $"Error al establecer la clave de comunicación." };

            }
            bool isConnected = _device.Connect_Net(configuracionHuellero.Ip, (int)(configuracionHuellero.Puerto));
            if (!isConnected)
            {
                return new ApiResponseDTO() { Success = false, Message = $"Error al conectar con el dispositivo ZKTeco." };

            }



            int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0;

            // Verificar si la conexión es funcional obteniendo la hora del dispositivo
            if (!_device.GetDeviceTime(machineNumber, ref year, ref month, ref day, ref hour, ref minute, ref second))
            {
                return new ApiResponseDTO() { Success = false, Message = $"Conexión fallida. No se pudo obtener la hora del dispositivo." };

            }

            _infoDevice.ipDevice = configuracionHuellero.Ip;
            _infoDevice.portDevice = configuracionHuellero.Puerto;


            // Registrar el evento para detectar entradas y salidas en tiempo real
            //device.OnAttTransactionEx += new _IZKEMEvents_OnAttTransactionExEventHandler(RegistrarEventoAsistencia);

            bool eventRegistered = _device.RegEvent(machineNumber, 65535); // Escuchar todos los eventos
            if (!eventRegistered)
            {
                return new ApiResponseDTO() { Success = false, Message = $"Error: No se pudo registrar el evento de asistencia." };

            }

            Console.WriteLine("Monitoreando eventos de asistencia en tiempo real...");

            return new ApiResponseDTO() { Success = true, Message = $"Dispositivo conectado exitosamente." };


        }

        public async Task<ApiResponseDTO> RecibirDatos(FiltroDatosDTO? datos)
        {
            if (ValidarConexion() != "Conectado al dispositivo")
            {
                return new ApiResponseDTO() { Success = false, Message = $"No se ha establecido una conexion previa con el dispositivo" };
            }

            ApiResponseDTO res = new ApiResponseDTO() { Success = true, Message = $"Usuarios cargados exitosamente." };


            string sql = @$"WITH CTE_Entradas AS (
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
                                             AND DATEADD(MINUTE, CONVERT(bigint, P.Value), H.HoraFin)
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
                                    S.Hora BETWEEN DATEADD(MINUTE, -CONVERT(bigint, P.Value), H.HoraInicio) 
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
                                (u.Tipo_Identificacion + CONVERT(VARCHAR(MAX), u.Identificacion) + ' - ' + u.Nombre) AS Empleado
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
                            ORDER BY 
                                e.IdUsuario, e.IdHorario, e.Fecha;";
            var usuarios = await _sqlServerDbContext.Database.GetDbConnection().QueryAsync<UsuarioBaseDTO?>(sql, new { idUsuario = datos?.IdUsuario, fechaInicio = datos?.FechaInicio, fechaFin = datos?.FechaFin });

            res.Data = usuarios.ToList();

            return res;

        }

        public async Task<ApiResponseDTO> AlimentarBase()
        {
            var sql = "";
            UsuarioHuelleroDTO nuevoUsuario = new UsuarioHuelleroDTO();
            if (ValidarConexion() != "Conectado al dispositivo")
            {
                return new ApiResponseDTO() { Success = false, Message = $"No se ha establecido una conexion previa con el dispositivo" };
            }
            ApiResponseDTO res = new ApiResponseDTO() { Success = true, Message = $"Informacion almacenada en la base correctamente." };

            string userID = "";
            int verifyMode = 0;
            int inOutMode = 0;
            int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0;
            int workCode = 0;
            bool hasRecords = false;
            string name = "";
            string password = "";
            int permisos = 0;
            bool activo = false;

            while (_device.SSR_GetGeneralLogData(machineNumber, out userID, out verifyMode, out inOutMode,
                                                out year, out month, out day, out hour, out minute, out second, ref workCode))
            {
                _device.SSR_GetUserInfo(machineNumber, userID, out name, out password, out permisos, out activo);

                DateTime fechaRegistro = new DateTime(year, month, day);

                hasRecords = true;

                nuevoUsuario = new UsuarioHuelleroDTO
                {
                    IdUsuario = userID,  // Asignar la identificación (puedes cambiar esto según tus necesidades)
                    Nombre = name,  // Asignar el nombre del usuario
                    Fecha = $"{year}-{month}-{day} {hour}:{minute}:{second}",  // Asignar el nombre del usuario
                    Tipo = (inOutMode == 0 ? "Entrada" : "Salida")  // Asignar el nombre del usuario
                };

                string identificacionUsuario = name.Split('-')[0];

                var IdUsuario = await ObtenerIdUsuario(identificacionUsuario);

                // Agregar el nuevo usuario a la lista
                if (nuevoUsuario.Tipo == "Entrada")
                {
                    sql = "INSERT INTO [Datos].Entradas (IdUsuario,Fecha,Hora) VALUES (@idUsuario,@fechaEntrada,@horaEntrada)";
                    var response = await _sqlServerDbContext.Database.GetDbConnection().ExecuteAsync(sql, new { idUsuario = IdUsuario, horaEntrada = $"{hour}:{minute}:{second}", fechaEntrada = $"{year}-{month}-{day}" });
                    //return new ApiResponseDTO() { Success = response > 0, Message = $"Usuario creado con exito!", Data = response };
                }
                else if (nuevoUsuario.Tipo == "Salida")
                {
                    sql = "INSERT INTO [Datos].Salidas (IdUsuario,Fecha,Hora) VALUES (@idUsuario,@fechaSalida,@horaSalida)";
                    var response = await _sqlServerDbContext.Database.GetDbConnection().ExecuteAsync(sql, new { idUsuario = IdUsuario, horaSalida = $"{hour}:{minute}:{second}", fechaSalida = $"{year}-{month}-{day}" });
                    //return new ApiResponseDTO() { Success = response > 0, Message = $"Usuario creado con exito!", Data = response };
                }

            }

            bool deleteSuccess = _device.ClearGLog(machineNumber); // Eliminar los registros
            if (!deleteSuccess)
            {
                res.Success = false;
                res.Message = $"Error al intentar eliminar los registros de asistencia.";
                return res;
            }

            if(nuevoUsuario is null)
            {
                res.Message = $"No hubo informacion que almacenar";

            }

            res.Data = nuevoUsuario;

            return res;

        }

        public bool CrearUsuario(string nombre, string identificacion, string password, int privilege, bool enabled)
        {
            _device.EnableDevice(1, false);
            //string nuevoId = ObtenerSiguienteIdUsuario(); // Generar el siguiente ID automáticamente

            bool setUserInfo = _device.SSR_SetUserInfo(
                1,               // Número de máquina
                identificacion,          // ID generado
                nombre,           // Nombre del usuario
                password,         // Contraseña
                privilege,        // Nivel de privilegio
                enabled           // Estado del usuario
            );

            if (!setUserInfo)
            {
                int errorCode = 0;
                _device.GetLastError(ref errorCode);
                Console.WriteLine($"Error al crear el usuario. Código de error: {errorCode}");
                _device.EnableDevice(1, true);

                return false;

            }

            Console.WriteLine($"Usuario creado con éxito. ID: {identificacion}, Nombre: {nombre}");
            
            _device.EnableDevice(1, true);

            return true;

        }

        private string ObtenerSiguienteIdUsuario()
        {
            int machineNumber = 1; // Número de la máquina
            string userId = "";
            string name = "";
            string password = "";
            int privilege = 0;
            bool enabled = false;

            int id = 1; // Comenzar desde el ID 1
            while (true)
            {
                bool existe = _device.SSR_GetUserInfo(machineNumber, id.ToString(), out name, out password, out privilege, out enabled);
                if (!existe) // Si no existe, este ID está disponible
                {
                    return id.ToString();
                }
                id++;
            }
        }

        public async Task<ApiResponseDTO> ObtenerRegistrosAsistenciaFiltrado(DateTime? fechaFiltrada)
        {

            if (ValidarConexion() != "Conectado al dispositivo")
            {
                return new ApiResponseDTO() { Success = false, Message = $"No se ha establecido una conexion previa con el dispositivo" };
            }
            ApiResponseDTO res = new ApiResponseDTO() { Success = true, Message = $"Registros de asistencia obtenidos exitosamente." };

            List<UsuarioHuelleroDTO> usuarios = new List<UsuarioHuelleroDTO>();

            string userID = "";
            int verifyMode = 0;
            int inOutMode = 0;
            int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0;
            int workCode = 0;
            bool hasRecords = false;
            string name = "";
            string password = "";
            int permisos = 0;
            bool activo = false;

            if (fechaFiltrada == DateTime.MinValue)
            {
                return new ApiResponseDTO() { Success = false, Message = $"No se ingreso una fecha" };

            }

            while (_device.SSR_GetGeneralLogData(machineNumber, out userID, out verifyMode, out inOutMode,
                                                out year, out month, out day, out hour, out minute, out second, ref workCode))
            {
                _device.SSR_GetUserInfo(machineNumber, userID, out name, out password, out permisos, out activo);

                DateTime fechaRegistro = new DateTime(year, month, day);

                if (fechaRegistro.Date == fechaFiltrada?.Date)
                {
                    hasRecords = true;

                    UsuarioHuelleroDTO nuevoUsuario = new UsuarioHuelleroDTO
                    {
                        IdUsuario = userID,  // Asignar la identificación (puedes cambiar esto según tus necesidades)
                        Nombre = name,  // Asignar el nombre del usuario
                        Fecha = $"{year}-{month}-{day} {hour}:{minute}:{second}",  // Asignar el nombre del usuario
                        Tipo = (inOutMode == 0 ? "Entrada" : "Salida")  // Asignar el nombre del usuario
                    };

                    // Agregar el nuevo usuario a la lista
                    usuarios.Add(nuevoUsuario);
                }


                
            }

            if (!hasRecords)
            {
                return new ApiResponseDTO() { Success = false, Message = $"No se encontraron registros para la fecha {fechaFiltrada?.ToString("yyyy-MM-dd")}." };

            }
            else
            {
                res.Data = usuarios;
            }

            return res;
        }

        public async Task<ApiResponseDTO> ObtenerRegistrosAsistencia()
        {
            var sql = "";
            if (ValidarConexion() != "Conectado al dispositivo")
            {
                return new ApiResponseDTO() { Success = false, Message = $"No se ha establecido una conexion previa con el dispositivo" };
            }
            ApiResponseDTO res = new ApiResponseDTO() { Success = true, Message = $"Registros de asistencia obtenidos exitosamente." };

            List<UsuarioHuelleroDTO> usuarios = new List<UsuarioHuelleroDTO>();

            string userID = "";
            int verifyMode = 0;
            int inOutMode = 0;
            int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0;
            int workCode = 0;
            bool hasRecords = false;
            string name = "";
            string password = "";
            int permisos = 0;
            bool activo = false;

            while (_device.SSR_GetGeneralLogData(machineNumber, out userID, out verifyMode, out inOutMode,
                                                out year, out month, out day, out hour, out minute, out second, ref workCode))
            {
                _device.SSR_GetUserInfo(machineNumber, userID, out name, out password, out permisos, out activo);

                DateTime fechaRegistro = new DateTime(year, month, day);

                hasRecords = true;

                UsuarioHuelleroDTO nuevoUsuario = new UsuarioHuelleroDTO
                {
                    IdUsuario = userID,  // Asignar la identificación (puedes cambiar esto según tus necesidades)
                    Nombre = name,  // Asignar el nombre del usuario
                    Fecha = $"{year}-{month}-{day} {hour}:{minute}:{second}",  // Asignar el nombre del usuario
                    Tipo = (inOutMode == 0 ? "Entrada" : "Salida")  // Asignar el nombre del usuario
                };
                // Agregar el nuevo usuario a la lista
                usuarios.Add(nuevoUsuario);
                

            }

            if (!hasRecords)
            {
                return new ApiResponseDTO() { Success = false, Message = $"No se encontraron registros." };

            }
            else
            {
                res.Data = usuarios;
            }

            return res;
        }

        private string ValidarConexion()
        {
            if (_device is null) { 
                return "No se ha configurado la ip del huellero";
            }
            bool passwordSet = _device.SetCommPassword(commPassword);
            if (!passwordSet)
            {
                return "Error al establecer la clave de comunicación.";
            }


            if (!_device.Connect_Net(_infoDevice.ipDevice, (int)_infoDevice.portDevice))
            {
                return "Error: No se pudo conectar al dispositivo a través de la red. Verifica la IP y el puerto.";
            }

            return "Conectado al dispositivo";
        }

        public string EstablecerFechaHoraDispositivo(DateTime nuevaFechaHora)
        {
            var resp = ValidarConexion();

            if (resp != "Conectado al dispositivo")
            {
                return "No se ha establecido una conexión previa con el dispositivo";
            }

            // Establecer la nueva fecha y hora en el dispositivo
            bool fechaHoraEstablecida = _device.SetDeviceTime2(machineNumber, nuevaFechaHora.Year, nuevaFechaHora.Month, nuevaFechaHora.Day, nuevaFechaHora.Hour, nuevaFechaHora.Minute, nuevaFechaHora.Second);

            if (!fechaHoraEstablecida)
            {
                return "Error al establecer la fecha y hora en el dispositivo.";
            }

            return "Fecha y hora del dispositivo establecidas exitosamente.";
        }

        public async Task<string> SincronizarFechaHoraConPC()
        {

            if (_infoDevice.ipDevice is null || _infoDevice.portDevice is null)
            {
                Console.WriteLine("Conectando...");
                await ConectarDispositivo();
            }
            Console.WriteLine($"Se ejecuto en el tiempo {DateTime.Now}");
            return EstablecerFechaHoraDispositivo(DateTime.Now);
        }

        public void IniciarSincronizacionPeriodica(int intervaloSegundos)
        {
            
            

            DetenerSincronizacionPeriodica();

            // Crear un nuevo token para la tarea de sincronización
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            // Iniciar la tarea de sincronización en segundo plano
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        // Llamar al método de sincronización
                        var resultado = await SincronizarFechaHoraConPC();
                        Console.WriteLine($"Sincronización realizada: {resultado}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error durante la sincronización: {ex.Message}");
                    }

                    // Esperar el intervalo antes de la siguiente ejecución
                    await Task.Delay(intervaloSegundos * 1000, token);
                }
            }, token);
        }

        public void DetenerSincronizacionPeriodica()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        private async Task<int?> ObtenerIdUsuario(string identificacion)
        {

            string sql = "SELECT Id FROM [Datos].Usuarios where Identificacion = @identificacion";
            var responseVerif = await _sqlServerDbContext.Database.GetDbConnection().QueryFirstOrDefaultAsync<int?>(sql, new { identificacion = identificacion });

            return responseVerif;
        }
    }
}
