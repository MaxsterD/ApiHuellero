using Blazored.SessionStorage;
using ConsolaBlazor.Services.DTOs.Login;
using ConsolaBlazor.Services.SessionStore.StoreSession;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ConsolaBlazor.Services.SessionStore
{
    public class AutenticacionExtension : AuthenticationStateProvider
    {
        private ClaimsPrincipal _sinInformacion = new ClaimsPrincipal(new ClaimsIdentity());

        private readonly ISessionStorageService _sessionStorageService;
        private IConfiguration Configuration;
        private readonly HttpClient _httpClient;
        private double timeExpiration;
        private readonly ClienteSession _userSessionService;

        public AutenticacionExtension(ISessionStorageService sessionStorageService, IConfiguration configuration, HttpClient httpClient, ClienteSession userSessionService)
        {
            _sessionStorageService = sessionStorageService;
            Configuration = configuration;
            _httpClient = httpClient;
            _userSessionService = userSessionService;
            //var timeExpirationSession = Configuration["TimeExpirationSession"];

            //if (!double.TryParse(timeExpirationSession, out timeExpiration))
            //{
            //    timeExpiration = 1;
            //}

        }

        public async Task ActualizarEstadoAutenticacion(SesionDTO? sesionUsuario)
        {
            ClaimsPrincipal claimsPrincipal;

            if (sesionUsuario != null)
            {
                sesionUsuario.Expiration = DateTime.UtcNow.AddMinutes(timeExpiration);
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name,sesionUsuario.Nombre),
                    new Claim("AccessToken", sesionUsuario.Token)
                }, "JwtAuth"));

                _userSessionService.SetuserSession(sesionUsuario);
                await _sessionStorageService.GuardarStorage("sesionUsuario", sesionUsuario, TimeSpan.FromMinutes(timeExpiration));
            }
            else
            {
                claimsPrincipal = _sinInformacion;
                await _sessionStorageService.RemoveItemAsync("sesionUsuario");
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var sesionUsuario = await _sessionStorageService.ObtenerStorage<SesionDTO>("sesionUsuario");


            if (sesionUsuario == null || sesionUsuario.Expiration < DateTime.UtcNow)
            {
                return await Task.FromResult(new AuthenticationState(_sinInformacion));
            }

            var claimPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name,sesionUsuario.Nombre),
                    new Claim("AccessToken", sesionUsuario.Token)
                }, "JwtAuth"));
            return await Task.FromResult(new AuthenticationState(claimPrincipal));
        }

    }
}
