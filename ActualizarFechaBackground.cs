using ApiConsola.Interfaces.ConexionHuellero;
using ApiConsola.Interfaces.Horarios;
using ApiConsola.Services.Horarios;

namespace ApiConsola
{
    public class ActualizarFechaBackground : BackgroundService
    {
        private readonly ILogger<ActualizarFechaBackground> _logger;
        private readonly IHuelleroService _conexionHuellero;
        private readonly int _intervaloSegundos = 60; // Intervalo de tiempo en segundos

        public ActualizarFechaBackground(ILogger<ActualizarFechaBackground> logger, IHuelleroService conexionHuellero)
        {
            _logger = logger;
            _conexionHuellero = conexionHuellero;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Mientras la aplicación esté en ejecución y el token de cancelación no se haya activado
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _conexionHuellero.ConectarDispositivo();
                    await _conexionHuellero.SincronizarFechaHoraConPC();
                    _logger.LogInformation("Ejecutando cron job a las {time}", DateTimeOffset.Now);

                    // Ejemplo: ejecutar un método que haga algo como sincronizar la hora o realizar alguna acción periódica
                    Console.WriteLine("Sincronización realizada a: " + DateTime.Now);

                    // Esperar el intervalo antes de la siguiente ejecución
                    await Task.Delay(_intervaloSegundos * 1000, stoppingToken); // Esperar por el intervalo
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error ejecutando cron job.");
                }
            }
        }
    }
}
