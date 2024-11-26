using ApiConsola.Services.Interfaces;
using System.Net.Http.Headers;

namespace ApiConsola.Services.Http
{
    public class EnviarHttp: IEnviarHttp
    {
        private readonly IConfiguration _configuration;

        public EnviarHttp(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ObtenerToken()
        {
            var token = _configuration["AuthorizationToken"] ?? "";
            return token;
        }

        public async Task<HttpResponseMessage> SendAsync(string url, HttpMethod method, HttpContent? content, string? accessToken = null)
        {
            accessToken = ObtenerToken();
            using(var client = new HttpClient())
            {
                var request = new HttpRequestMessage(method, url) { Content = content};

                if(!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", accessToken);
                }

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return await client.SendAsync(request);
            }
        }
    }
}
