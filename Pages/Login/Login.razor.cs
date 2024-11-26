using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ConsolaBlazor.Services.SessionStore;
using System.Net.Http.Json;
using System.Text.Json;
using ConsolaBlazor.Services.DTOs.Login;
using Microsoft.AspNetCore.Components.Web;

namespace ConsolaBlazor.Pages.Login
{
    public partial class Login
    {
        public string TextValue { get; set; }
        [Inject] HttpClient httpClient { get; set; }
        [Inject] AuthenticationStateProvider autenticacionProvider { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IConfiguration Configuration { get; set; }

        private LoginDTO login = new LoginDTO();

        private bool isShow;
        private InputType PasswordInput = InputType.Password;
        private string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

        void ButtonTestclick()
        {
            if (isShow)
            {
                isShow = false;
                PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                PasswordInput = InputType.Password;
            }
            else
            {
                isShow = true;
                PasswordInputIcon = Icons.Material.Filled.Visibility;
                PasswordInput = InputType.Text;
            }
        }

        private async Task IniciarSesion()
        {
            if (login == null || string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
            {
                Snackbar.Add("Por favor, introduce tus datos de inicio de sesión.", Severity.Warning, config => { config.HideIcon = true; });
                return;
            }

            try
            {
                var baseUrl = Configuration["UrlBackend"];
                var loginResponse = await httpClient.PostAsJsonAsync($"{baseUrl}/api/Login/autenticacion", login);
                

                if (loginResponse.IsSuccessStatusCode)
                {
                    var sesionUsuario = await loginResponse.Content.ReadFromJsonAsync<SesionDTO>();

                    var autenticacionExitosa = (AutenticacionExtension)autenticacionProvider;


                    await autenticacionExitosa.ActualizarEstadoAutenticacion(sesionUsuario);
                    Snackbar.Add("Autenticacion exitosa", Severity.Success, config => { config.HideIcon = true; });
                    Navigation.NavigateTo("/administrarTiempos");
                }
                else if (loginResponse.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errorResponse = await loginResponse.Content.ReadAsStringAsync();
                    var errorData = JsonSerializer.Deserialize<LoginMessage>(errorResponse);

                    Snackbar.Add(errorData?.Message, Severity.Warning, config => { config.HideIcon = true; });
                }
            }
            catch (Exception e)
            {
                Snackbar.Add($"Error en el servidor: {e.Message}", Severity.Error, config => { config.HideIcon = true; });
            }
        }

        private async Task HandleTicketSearch(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await IniciarSesion();
            }
        }

    }
}
