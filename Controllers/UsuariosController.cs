using ApiConsola.Interfaces.AsignarLideres;
using ApiConsola.Interfaces.CreacionUsuario;
using ApiConsola.Services.DTOs;
using ApiConsola.Services.DTOs.AsignarLideres;
using Microsoft.AspNetCore.Mvc;

namespace ApiConsola.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ICreacionUsuarioService _usuarioService;

        public UsuariosController(ICreacionUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("CrearUsuario")]
        public async Task<IActionResult> CrearUsuario(UsuarioDTO datos)
        {
            var session = await _usuarioService.CrearUsuario(datos);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No fue posible crear el usuario" });
        }

        [HttpGet("BuscarUsuario")]
        public async Task<IActionResult> BuscarUsuario(UsuarioDTO datos)
        {
            var session = await _usuarioService.BuscarUsuario(datos);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existen el usuario" });
        }

        [HttpGet("ListarUsuarios")]
        public async Task<IActionResult> ListarUsuarios([FromQuery] UsuarioDTO? datos = null)
        {
            var session = await _usuarioService.ListarUsuarios(datos);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existen usuarios creados" });
        }

        [HttpPost("EliminarUsuario")]
        public async Task<IActionResult> EliminarUsuario(UsuarioDTO datos)
        {
            var session = await _usuarioService.EliminarUsuario((int)datos.Id);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = "No existen usuarios creados" });
        }
    }
}
