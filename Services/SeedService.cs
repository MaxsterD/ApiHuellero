using ApiConsola.Interfaces;
using ApiConsola.Services.DTOs;
using ApiConsola.Services.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace ApiConsola.Services
{
    public class SeedService:ISeedService
    {
        private readonly IConfiguration _configuration;
        private readonly IEnviarHttp _enviarHttp;
        public SeedService(IConfiguration configuration, IEnviarHttp enviarHttp)
        {
            _configuration = configuration;
            _enviarHttp = enviarHttp;
        }

        public async Task<SeedResponse> FillDB()
        {
            string jsonPath = "./DataClientes.json";
            List<NewUsuarioDTO> usuarios = new List<NewUsuarioDTO>();
            try
            {
                using(StreamReader reader = new StreamReader(jsonPath))
                {
                    var json = await reader.ReadToEndAsync();
                    usuarios = JsonConvert.DeserializeObject<List<NewUsuarioDTO>>(json);
                }

                string url = "http://10.120.4.2:98/api/Login/Registrar";
                //string url = "http://localhost:5244/api/Login/Registrar";

                int TotalCreados = 0;
                List<NewUsuarioDTO> NoCreados = new List<NewUsuarioDTO>();

                foreach (var user in usuarios)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    var response = await _enviarHttp.SendAsync(url, HttpMethod.Post, content);

                    if (response.IsSuccessStatusCode)
                    {
                        TotalCreados++;
                    }
                    else
                    {
                        NoCreados.Add(user);
                    }
                }
                SeedResponse seedResponse = new()
                {
                    NoCreados = NoCreados,
                    TotalCreados = TotalCreados,
                };
                return seedResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
