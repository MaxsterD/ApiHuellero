using ApiConsola.Interfaces;
using ApiConsola.Services.DTOs;
using ApiConsola.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Text;

namespace ApiConsola.Services.CrearTicket
{
    public class CrearTicketService: ICrearTicketService
    {
        private readonly IEnviarHttp _enviarHttp;
        private readonly IConfiguration _configuration;

        public CrearTicketService(IEnviarHttp enviarHttp, IConfiguration configuration)
        {
            _enviarHttp = enviarHttp;
            _configuration = configuration;
        }

        public async Task<ApiResponseDTO> CrearTicket(BodyTicketRequest ticket)
        {
            if(ValidarTicket(ticket))
            {
                var baseUrl = _configuration["AzureApi"];
                var organizacion = _configuration["Organizacion"];
                var proyecto = _configuration["Proyecto"];
                var sprint = _configuration["Sprint"];
                var tablero = _configuration["Tablero"];

                try
                {
                    List<ItemNewTicket> newTicket = MapTicket(ticket).NewTicket;
                    var content = new StringContent(JsonConvert.SerializeObject(newTicket), Encoding.UTF8, "application/json-patch+json");

                    var url = $"{baseUrl}/{organizacion}/{proyecto}/_apis/wit/workitems/$Support?api-version=6.0";

                    var response = await _enviarHttp.SendAsync(url, HttpMethod.Post, content);

                    if(response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var jsonData = JsonConvert.DeserializeObject<TicketResponse>(data);

                        var result = new ApiResponseDTO()
                        {
                            Success = true,
                            Message = "Ticket Creado con éxito",
                            Data = new {NroTicket = jsonData?.id}
                        };

                        return result;
                    }
                    else
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        return new ApiResponseDTO() { Success = false, Message = $"Error: {response.StatusCode}, Detalle: {responseData}" };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new ApiResponseDTO() { Success = false, Message = ex.Message };
                }

            }
            return new ApiResponseDTO() { Success = false, Message = "Datos del ticket incompletos"};
        }

        private bool ValidarTicket(BodyTicketRequest ticket)
        {
            return (ticket != null
                && !string.IsNullOrEmpty(ticket.Titulo)
                && !string.IsNullOrEmpty(ticket.Descripcion)
                && !string.IsNullOrEmpty(ticket.Cliente));
        }

        private NewTicketDTO MapTicket(BodyTicketRequest ticket)
        {

            var proyecto = _configuration["Proyecto"];
            var sprint = _configuration["Sprint"];
            var tablero = _configuration["Tablero"];

            var title = new ItemNewTicket()
            {
                path = "/fields/System.Title",
                value = ticket.Titulo
            };

            var description = new ItemNewTicket()
            {
                path = "/fields/System.Description",
                value = ticket.Descripcion
            };

            var tableroPath = new ItemNewTicket()
            {
                path = "/fields/System.AreaPath",
                value = $"{proyecto}\\{tablero ?? "Developers"}"
            };

            var sprintPath = new ItemNewTicket()
            {
                path = "/fields/System.IterationPath",
                value = $"{proyecto}\\{sprint ?? "Sprint 1"}"
            };

            var asignacion = new ItemNewTicket()
            {
                path = "/fields/System.AssignedTo",
                value = ticket.Asignacion
            };

            var cliente = new ItemNewTicket()
            {
                path = "/fields/Custom.Cliente",
                value = ticket.Cliente
            };

            var newTicket = new NewTicketDTO()
            {
                NewTicket = new List<ItemNewTicket>
                {
                    title, description, tableroPath, sprintPath, asignacion, cliente
                }
            };

            return newTicket;
        }

    }
}
