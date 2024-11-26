using ApiConsola.Interfaces;
using ApiConsola.Services.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiConsola.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComentariosController: ControllerBase
    {
        private readonly ICrearComentarioService _crearComentarioService;

        public ComentariosController(ICrearComentarioService crearComentarioService)
        {
            _crearComentarioService = crearComentarioService;
        }

        [HttpPatch("CrearComentario")]
        public async Task<IActionResult> CrearComentario(NewComentarioDTO Comentario)
        {
            var response = await _crearComentarioService.CrearComentario(Comentario);

            if(response == null)
            {
                return BadRequest( new {Message = "Error al crear el comentario"});
            }
            else if(!response.Success)
            {
                return BadRequest(new { Message = response.Message });
            }

            return Ok(response);
        }
    }
}
