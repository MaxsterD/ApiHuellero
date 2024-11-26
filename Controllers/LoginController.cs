using ApiConsola.Interfaces;
using ApiConsola.Services.DTOs;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiConsola.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserServices _createUserServices;

        public LoginController(IUserServices createUserServices)
        {
            _createUserServices = createUserServices;
        }

        [HttpPost("autenticacion")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            var session = await _createUserServices.Login(login);

            if (session != null)
            {
                return Ok(session);
            }
            return BadRequest(new {Message = "Credenciales invalidas"});
        }

        [HttpPost("Registrar")]
        public async Task<IActionResult> Registrar(NewUsuarioDTO newLogin)
        {
            var session = await _createUserServices.RegistrarUsuario(newLogin);

            if (session.Success)
            {
                return Ok(new {Message = session.Message});
            }
            return BadRequest(new { Message = session.Message });
        }

        [HttpPost("CambiarPassword")]
        public async Task<IActionResult> CambiarPassword(UpdateDTO newData)
        {
            var session = await _createUserServices.Actualizar(newData);

            if (session.Success)
            {
                return Ok(new { Message = session.Message });
            }
            return BadRequest(new { Message = "No se pudo actualizar la contraseña" });
        }

        [HttpPatch("Actualizar")]
        public async Task<IActionResult> Actualizar(UpdateData newLogin)
        {
            var session = await _createUserServices.ActualizarUsuario(newLogin);

            if (session.Success)
            {
                return Ok(new { Message = session.Message });
            }
            return BadRequest(new { Message = session.Message });
        }

        [HttpGet("BuscarUsuario")]
        public async Task<IActionResult> BuscarUsuario(string email)
        {
            var session = await _createUserServices.BuscarUsuario(email);

            if (session.Success)
            {
                return Ok(session);
            }
            return BadRequest(new { Message = session.Message });
        }
    }
}
