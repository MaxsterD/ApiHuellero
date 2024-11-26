using ApiConsola.Interfaces;
using ApiConsola.Services.DTOs;
using ApiConsola.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace ApiConsola.Services.ConsultarTicket
{
    public class ConsultarTicketService: ITicketService
    {
        private readonly IConfiguration _configuration;
        private readonly IEnviarHttp _enviarHttp;
        public ConsultarTicketService(IConfiguration configuration, IEnviarHttp enviarHttp)
        {
            _configuration = configuration;
            _enviarHttp = enviarHttp;
        }

        public async Task<TicktDTO?> GetTicket(int numTicket)
        {
            var nombre = _configuration["Organizacion"];
            var grupo = _configuration["Proyecto"];
            var path = _configuration["AzureApi"];

            try
            {
                var urlEstado = $"{path}/{nombre}/{grupo}/_apis/wit/workitems/{numTicket}?api-version=6.0";
                var urlComentarios = $"{path}/{nombre}/{grupo}/_apis/wit/workitems/{numTicket}/comments?api-version=6.0-preview";

                var responseEstado = await _enviarHttp.SendAsync(urlEstado, HttpMethod.Get, null);
                var responseComentario = await _enviarHttp.SendAsync(urlComentarios, HttpMethod.Get, null);

                if(responseEstado.IsSuccessStatusCode && responseComentario.IsSuccessStatusCode)
                {
                    var responseBodyEstado = await responseEstado.Content.ReadAsStringAsync();
                    var responseBodyComentario = await responseComentario.Content.ReadAsStringAsync();

                    var jsonEstado = JsonConvert.DeserializeObject<EstadoTicketResponseDTO>(responseBodyEstado);
                    var jsonComentario = JsonConvert.DeserializeObject<ComentariosTicketResponseDTO>(responseBodyComentario);

                    TicktDTO ticket = TicketMap(jsonEstado, jsonComentario);

                    return ticket ?? null;
                   
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private TicktDTO TicketMap (EstadoTicketResponseDTO jsonEstado, ComentariosTicketResponseDTO jsonComentario)
        {
            var comentariosResponse = jsonComentario.comments.ToList();
            List<ComentariosTicket> ListaComentarios = new List<ComentariosTicket>();

            foreach(var comentario in comentariosResponse)
            {
                var nombreCreador = "";
                var comentarioReal = "";
                bool isAtower = !comentario.text.StartsWith("[");


                if(!isAtower)
                {
                    var first = comentario.text.Split(":")[0];
                    nombreCreador = first.Substring(1, first.Length - 2 );
                    comentarioReal = comentario.text.Split(":")[1];
                }
                else
                {
                    nombreCreador = comentario.createdBy.displayName;
                    comentarioReal = comentario.text;
                }

                ComentariosTicket comentariosData = new ComentariosTicket
                {
                    Comentario = comentarioReal,
                    Fecha = comentario.createdDate,
                    Nombre = nombreCreador,
                    Atower = isAtower,
                };

                ListaComentarios.Add( comentariosData );
            }


            TicktDTO ticktDTO = new TicktDTO
            {
                NroTicket = jsonEstado.id ?? 0,
                Consultor = jsonEstado.fields?.SAssignedTo?.displayName ?? "No asignado",
                Estado = jsonEstado?.fields?.SState ?? "Sin estado",
                Titulo = jsonEstado?.fields?.STitle ?? "",
                Descripcion = jsonEstado?.fields?.SDescription ?? "Sin Descripción",
                Comentarios = ListaComentarios,
            };

            return ticktDTO;
        }
        
    }
}
