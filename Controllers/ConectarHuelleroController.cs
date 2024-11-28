using ApiConsola.Interfaces.ConexionHuellero;
using ApiConsola.Services.DTOs.ConexionHuellero;
using Microsoft.AspNetCore.Mvc;

namespace ApiConsola.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConectarHuelleroController : ControllerBase
    {
        private readonly IConexionHuelleroService _conexionHuellero;

        public ConectarHuelleroController(IConexionHuelleroService conexionHuellero)
        {
            _conexionHuellero = conexionHuellero;
        }

        [HttpGet("ConexionDispositivo")]
        public async Task<IActionResult> ConectarDispositivo()
        {
            var response = await _conexionHuellero.ConectarDispositivo();

            return Ok(response);
        }

        [HttpPost("RecibirDatos")]
        public async Task<IActionResult> RecibirDatos(FiltroDatosDTO? datos = null)
        {
            var response = await _conexionHuellero.RecibirDatos(datos);

            return Ok(response);
        }

        [HttpGet("RecibirRegistrosFiltrado")]
        public async Task<IActionResult> RecibirRegistros(DateTime fecha)
        {
            var response =  await _conexionHuellero.ObtenerRegistrosAsistenciaFiltrado(fecha);

            return Ok(response);
        }

        [HttpGet("RecibirRegistros")]
        public async Task<IActionResult> RecibirRegistros()
        {
            var response = await _conexionHuellero.ObtenerRegistrosAsistencia();

            return Ok(response);
        }

        [HttpGet("EstablecerFechaHoraDispositivo")]
        public async Task<IActionResult> EstablecerFechaHoraDispositivo(DateTime fecha)
        {
            var response = _conexionHuellero.EstablecerFechaHoraDispositivo(fecha);

            return Ok(response);
        }

        [HttpGet("EstablecerFechaHoraDispositivoPC")]
        public async Task<IActionResult> EstablecerFechaHoraDispositivoPC()
        {
            var response = await _conexionHuellero.SincronizarFechaHoraConPC();

            return Ok(response);
        }

        [HttpGet("AlimentarBase")]
        public async Task<IActionResult> AlimentarBase()
        {
            var response = await _conexionHuellero.AlimentarBase();

            return Ok(response);
        }
    }
}
