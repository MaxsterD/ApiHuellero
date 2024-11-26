﻿using ApiConsola.Interfaces.CreacionUsuario;
using ApiConsola.Services.DTOs.Horarios;
using Microsoft.AspNetCore.Mvc;

namespace ApiConsola.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HorariosController : ControllerBase
    {
        private readonly IHorariosService _horariosService;

        public HorariosController(IHorariosService horariosService)
        {
            _horariosService = horariosService;
        }

        [HttpPost("CrearHorario")]
        public async Task<IActionResult> CrearHorario(HorariosDTO datos)
        {
            var session = await _horariosService.CrearHorario(datos);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No fue posible crear el horario" });
        }

        [HttpGet("BuscarHorario")]
        public async Task<IActionResult> BuscarHorario(HorariosDTO datos)
        {
            var session = await _horariosService.BuscarHorario(datos);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existen el horario" });
        }

        [HttpGet("ListarHorarios")]
        public async Task<IActionResult> ListarHorarios()
        {
            var session = await _horariosService.ListarHorarios();

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existen horarios creados" });
        }

        [HttpPost("EliminarHorario")]
        public async Task<IActionResult> EliminarHorario(HorariosDTO datos)
        {
            var session = await _horariosService.EliminarHorario((int)datos.Id);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existen horarios creados" });
        }
    }
}
