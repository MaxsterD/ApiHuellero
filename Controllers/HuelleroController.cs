﻿using ApiConsola.Interfaces.ConexionHuellero;
using ApiConsola.Services.DTOs.ConexionHuellero;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiConsola.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HuelleroController : ControllerBase
    {
        private readonly IHuelleroService _conexionHuellero;

        public HuelleroController(IHuelleroService conexionHuellero)
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
        
        [HttpPost("BorrarRegistro")]
        public async Task<IActionResult> BorrarRegistro(UsuarioBaseDTO? datos)
        {
            var response = await _conexionHuellero.BorrarRegistro(datos);

            return Ok(response);
        }

        [HttpPost("ActualizarRegistro")]
        public async Task<IActionResult> ActualizarRegistro(UsuarioBaseDTO? datos)
        {
            var response = await _conexionHuellero.ActualizarRegistro(datos);

            return Ok(response);
        }


        [HttpPost("CrearUsuario")]
        public async Task<IActionResult> CrearUsuario(string nombre, string identificacion, string password, int privilege = 0, bool enabled = true)
        {
            var response = await _conexionHuellero.CrearUsuario(nombre, identificacion, password, privilege, enabled);

            return Ok(response);
        }

        [HttpPost("CrearUsuarioPrueba")]
        public async Task<IActionResult> CrearUsuarioPrueba()
        {
            var response = await _conexionHuellero.CrearUsuarioPrueba();

            return Ok(response);
        }

        [HttpPost("ObtenerUsuariosHuellero")]
        public async Task<IActionResult> ObtenerUsuariosHuellero()
        {
            var response = _conexionHuellero.ObtenerUsuariosHuellero();

            return Ok(response);
        }

        [HttpGet("ArchivoRaspBerryPrueba")]
        public async Task<IActionResult> ArchivoRaspBerry(string datos)
        {
            var response = await _conexionHuellero.ArchivoRaspBerry(datos);

            return Ok(response);
        }
    }
}
