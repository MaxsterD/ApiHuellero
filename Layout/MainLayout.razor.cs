using ConsolaBlazor.Services.SessionStore;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using static MudBlazor.CategoryTypes;
using ConsolaBlazor.CustomStyle;
using ConsolaBlazor.Services.DTOs;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Net.Http.Json;
using System.Net.Http;
using System.Reflection;
using ConsolaBlazor.Components.Utils;

namespace ConsolaBlazor.Layout
{
    public partial class MainLayout
    {
        [Inject] HttpClient HttpClient { get; set; }
        [Inject] IConfiguration Configuration { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }

        [Inject]
        private AuthenticationStateProvider autenticacionProvider { get; set; }

        [Inject]
        private NavigationManager Navigation { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> authenticationState { get; set; }
        private bool _isDarkMode { get; set; }

        private NavMenuAtower? navMenuRef { get; set; }

        private async Task CerrarSesion()
        {
            var autenticacionExt = (AutenticacionExtension)autenticacionProvider;                        
            await autenticacionExt.ActualizarEstadoAutenticacion(null);
            Navigation.NavigateTo("/");
        }

        private void OpenNavMenuDrawer()
        {
            navMenuRef?.OpenDrawers();
        }

        //private async Task OpenDialog()
        //{
        //    var ParametrosModal = new LupaParametrosDTO
        //    {
        //        ParametrosBusqueda = new List<string>() { "Titulo", "NroTicket" },
        //        ColumnasTabla = GetColumnasTabla(new TicketDTO()),
        //        Data = (await FetchTickets())?.Cast<object>().ToList()
        //    };

        //    var parameters = new DialogParameters<BuscarLupa> {
        //        {x => x.Parametros, ParametrosModal }
        //    };

        //    var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth=true };
        //    await Dialog.ShowAsync<BuscarLupa>("Buscar Lupa", parameters, options);
        //}

        public async Task<List<TicketDTO>?> FetchTickets()
        {
            try
            {
                var BaseUrl = Configuration["UrlBackend"];
                var url = $"http://localhost:5244/api/Ticket/GetTicketsByClient?client=ebenezer";
                var response = await HttpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var ticketsResponse = await response.Content.ReadFromJsonAsync<List<TicketDTO>>();
                    if (ticketsResponse?.Count > 0)
                    {
                        return ticketsResponse;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Snackbar.Add("Hubo un error al cargar los tickets", Severity.Error, config => { config.HideIcon = true; });
            }
            return null;
        }

        public List<string> GetColumnasTabla(object? objectDTO)
        {
            Type DtoType = objectDTO.GetType();
            PropertyInfo[] propiedades = DtoType.GetProperties();

            var columnasTabla = new List<string>();
            foreach (PropertyInfo pi in propiedades)
            {
                columnasTabla.Add(pi.Name);
            }

            return columnasTabla;
        }
        public class TicketDTO
        {
            public int NroTicket { get; set; }
            public string Titulo { get; set; } = string.Empty;
            public string Estado { get; set; } = string.Empty;
            public string FechaCreacion { get; set; } = string.Empty;
            public string? FechaCierre { get; set; } = null;
        }
    }
}
