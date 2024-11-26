using ApiConsola.Interfaces;
using ApiConsola.Services.DTOs;
using ApiConsola.Services.Interfaces;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace ApiConsola.Services.ConsultarTicket
{
    public class ConsultarByClienteService:ITicketByClienteService
    {
        private readonly IConfiguration _configuration;
        private readonly IEnviarHttp _enviarHttp;
        public ConsultarByClienteService(IConfiguration configuration, IEnviarHttp enviarHttp)
        {
            _configuration = configuration;
            _enviarHttp = enviarHttp;
        }
        public async Task<List<TicketByClienteDTO>?> GetTicketByCliente(string cliente, List<string>? estados)
        {
            var nombre = _configuration["Organizacion"];
            var grupo = _configuration["Proyecto"];
            var path = _configuration["AzureApi"];

            try
            {
                string filterStates = "AND (";

                if(estados.Count > 0)
                {
                    foreach(var estado in estados)
                    {
                        filterStates = filterStates + $"[System.State] ='{estado}' OR ";
                    }
                    filterStates = filterStates.Substring(0, filterStates.Length - 3) + ")";
                }
                else
                {
                    filterStates = "";
                }


                var url = $"{path}/{nombre}/{grupo}/_apis/wit/wiql?api-version=6.0";
                BodyDTO body = new()
                {
                    query = $"SELECT [System.Id], [System.Title], [Custom.Cliente],[System.State] FROM workitems WHERE [Custom.Cliente] = '{cliente}' {filterStates} ORDER BY [System.Id] DESC"
                };

                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                var response = await _enviarHttp.SendAsync(url, HttpMethod.Post, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var jsonBody = JsonConvert.DeserializeObject<TicketByClienteResponseDTO>(responseBody);

                    List<TicketByClienteDTO> datos = new List<TicketByClienteDTO>();

                    foreach (var item in jsonBody.workItems)
                    {
                        var result = await GetSingleTicket(item.url);
                        datos.Add(result);
                    }

                    return datos;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }


            return null;
        }

        private class BodyDTO
        {
            public string query { get; set; } = string.Empty;
        }

        private async Task<TicketByClienteDTO?> GetSingleTicket(string url)
        {
            var response = await _enviarHttp.SendAsync(url, HttpMethod.Get, null);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonConvert.DeserializeObject<SingleTicketByClientRespone>(responseBody);

                TicketByClienteDTO result = new TicketByClienteDTO()
                {
                    NroTicket = jsonResponse.id,
                    Estado = jsonResponse.fields.SState,
                    Titulo = jsonResponse.fields.STitle,
                    FechaCreacion = ParseDate(jsonResponse.fields.SCreatedDate) ?? "",
                    FechaCierre = ParseDate(jsonResponse.fields.ClosedDate) ?? null,
                };
                return result;
            }

            return null;
        }

        private string? ParseDate(string? date)
        {
            if(date == null)
                return null;

            DateTime dateTime = DateTime.Parse(date, null, DateTimeStyles.AssumeUniversal);
            return dateTime.ToString("yyyy-MM-dd hh:mm");
        }

    }
}
