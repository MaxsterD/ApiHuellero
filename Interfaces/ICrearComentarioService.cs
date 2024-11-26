using ApiConsola.Services.DTOs;

namespace ApiConsola.Interfaces
{
    public interface ICrearComentarioService
    {
        Task<CreateResponse?> CrearComentario(NewComentarioDTO comentario);
    }
}
