using ApiConsola.Interfaces;
using ApiConsola.Services.DTOs;
using ApiConsola.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Text;

namespace ApiConsola.Services
{
    public class CrearComentarioService : ICrearComentarioService
    {
        private readonly IConfiguration _configuration;
        private readonly IEnviarHttp _enviarHttp;

        public CrearComentarioService(IConfiguration configuration, IEnviarHttp enviarHttp)
        {
            _configuration = configuration;
            _enviarHttp = enviarHttp;
        }

        public async Task<CreateResponse?> CrearComentario(NewComentarioDTO comentario)
        {
            try
            {
                var nombre = _configuration["Organizacion"];
                var grupo = _configuration["Proyecto"];
                var path = _configuration["AzureApi"];

                if (comentario != null && !string.IsNullOrEmpty(comentario.Comentario))
                {
                    var url = $"{path}/{nombre}/{grupo}/_apis/wit/workitems/{comentario.IdTicket}?api-version=6.0";
                    var body = new List<BodyQuery>()
                    {
                        new BodyQuery()
                        {
                            value = $"[{comentario.Usuario}]: {comentario.Comentario}"
                        }
                    };

                    var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json-patch+json");
                    
                    var response = await _enviarHttp.SendAsync(url, HttpMethod.Patch, content);

                    if(response.IsSuccessStatusCode)
                    {
                        CreateResponse result = new()
                        {
                            Success = true,
                            Message = "Comentario creado con éxito"
                        };
                        return result;
                    }
                    else
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var jsonContent = JsonConvert.DeserializeObject<ErrorJsonData>(responseContent);

                        CreateResponse result = new()
                        {
                            Success = false,
                            Message = jsonContent.message
                        };
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            return null;
        }

        private class BodyQuery
        {
            public string op { get; set; } = "add";
            public string path { get; set; } = "/fields/System.History";
            public string value { get ; set; }
        }

        private class ErrorJsonData
        {
            public string message { get; set; }
        }
    }
}
