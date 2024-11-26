using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ConsolaBlazor.Services.SessionStore;
using System.Net.Http.Json;
using System.Text.Json;
using ConsolaBlazor.Services.DTOs.Login;
using Microsoft.AspNetCore.Components.Web;
using ConsolaBlazor.CustomStyle;
using ConsolaBlazor.Services.DTOs.Horarios;
using static MudBlazor.CategoryTypes;
using ConsolaBlazor.Pages.Login;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using ConsolaBlazor.Components.Modales;
using Newtonsoft.Json;
using System.Text;
using ConsolaBlazor.Services.DTOs.CreacionUsuario;

namespace ConsolaBlazor.Pages.Horarios
{
    public partial class Horarios
    {
        private HorariosDTO horario = new HorariosDTO();
        private List<HorariosDTO> Horarios { get; set; } = new List<HorariosDTO>();

        [Inject] HttpClient httpClient { get; set; }
        [Inject] AuthenticationStateProvider autenticacionProvider { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IConfiguration Configuration { get; set; }

        private string titleBarStyle = $"height:7%;background-color:{AtowerTheme.Default.PaletteLight.Primary}; color:white;";
        private string headerBarStyle = $"background-color:{AtowerTheme.Default.PaletteLight.Primary}; color:white;";
        private MudTimePicker horaInicio;
        private MudTimePicker horaFin;
        

        private async Task DeleteServerAsync(TableRowData datos)
        {
            var parameters = new DialogParameters<ConfirmActionModal> { { x => x.Server, datos } };

            var dialog = await DialogService.ShowAsync<ConfirmActionModal>("Delete Server", parameters);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                //In a real world scenario we would reload the data from the source here since we "removed" it in the dialog already.

                Console.WriteLine(result.Data.ToString());
                
                tableData.RemoveAll(item => item.Column1 == (int)result.Data);
            }
        }

        private void Prueba()
        {
            if (horario == null)
            {
                Snackbar.Add("Por favor, introduce los datos del horario.", Severity.Warning, config => { config.HideIcon = true; });
                return;
            }else if (string.IsNullOrEmpty(horario.Descripcion))
            {
                Snackbar.Add("Por favor, introduce una Descripción.", Severity.Warning, config => { config.HideIcon = true; });
                return;
            }
            else if (string.IsNullOrEmpty(horario.HoraInicio))
            {
                Snackbar.Add("Por favor, introduce una Hora Inicio.", Severity.Warning, config => { config.HideIcon = true; });
                return;
            }
            else if (string.IsNullOrEmpty(horario.HoraFin))
            {
                Snackbar.Add("Por favor, introduce una Hora Fin.", Severity.Warning, config => { config.HideIcon = true; });
                return;
            }
            
            tableData.Add(new TableRowData{ Column1 = 1, Column2 = horario.Descripcion, Column3 = horario.HoraInicio, Column4 = horario.HoraFin });

            horario.Descripcion = "";
            horaInicio.ClearAsync();
            horaFin.ClearAsync();

        }

        private async Task Guardar()
        {
            if (horario == null)
            {
                Snackbar.Add("Por favor, introduce los datos del horario.", Severity.Warning, config => { config.HideIcon = true; });
                return;
            }
            else if (string.IsNullOrEmpty(horario.Descripcion))
            {
                Snackbar.Add("Por favor, introduce una Descripción.", Severity.Warning, config => { config.HideIcon = true; });
                return;
            }
            else if (string.IsNullOrEmpty(horario.HoraInicio))
            {
                Snackbar.Add("Por favor, introduce una Hora Inicio.", Severity.Warning, config => { config.HideIcon = true; });
                return;
            }
            else if (string.IsNullOrEmpty(horario.HoraFin))
            {
                Snackbar.Add("Por favor, introduce una Hora Fin.", Severity.Warning, config => { config.HideIcon = true; });
                return;
            }

            var myContent = JsonConvert.SerializeObject(horario);
            var content = new StringContent(myContent, Encoding.UTF8, "application/json");
            var baseUrl = Configuration["UrlBackend"];
            var url = $"{baseUrl}/api/CreacionUsuario/CrearUsuario";
            var response = await httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {

                await FetchHorarios().ConfigureAwait(false);

                Snackbar.Add("Horario creado con exito!", Severity.Success);

                horario.Descripcion = "";
                await horaInicio.ClearAsync();
                await horaFin.ClearAsync();


            }
            else
            {
                await FetchHorarios().ConfigureAwait(false);

                Snackbar.Add("Hubo un error al crear el horario!", Severity.Error);

            }



        }

        private async Task BorrarHorario(HorariosDTO datos)
        {
            var parameters = new DialogParameters<ModalBorrarHorario> { { x => x.Server, datos } };

            var dialog = await DialogService.ShowAsync<ModalBorrarHorario>("Borrar Horario", parameters);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                //In a real world scenario we would reload the data from the source here since we "removed" it in the dialog already.

                Console.WriteLine(result.Data.ToString());
                Console.WriteLine($"Event = Delete server, Data = {System.Text.Json.JsonSerializer.Serialize(result.Data)}");

                await FetchHorarios();
            }
        }

        private async Task ActualizarHorario(HorariosDTO item)
        {
            var myContent = JsonConvert.SerializeObject(item);
            var content = new StringContent(JsonConvert.SerializeObject(myContent), Encoding.UTF8, "application/json");
            var baseUrl = Configuration["UrlBackend"];
            var url = $"{baseUrl}/api/CreacionUsuario/ActualizarUsuario";
            var response = await httpClient.PostAsync(url, content);
            Console.WriteLine($"Event = CommittedItemChanges, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
            Console.WriteLine(response.ToString());
        }

        private async Task FetchHorarios()
        {
            try
            {
                Console.WriteLine("Traer usuarios");
                var baseUrl = Configuration["UrlBackend"];
                var url = $"{baseUrl}/api/CreacionUsuario/ListarUsuarios";
                var response = await httpClient.GetAsync(url);
                Console.WriteLine("RespUsuarios");
                Console.WriteLine(response.ToString());
                if (response.IsSuccessStatusCode)
                {
                    var horarios = await response.Content.ReadFromJsonAsync<List<HorariosDTO>>();
                    if (horarios?.Count > 0)
                    {
                        Horarios = horarios;

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Snackbar.Add("Hubo un error al traer los usuarios", Severity.Error, config => { config.HideIcon = true; });
            }
        }

        // Lista de datos de la tabla
        private List<TableRowData> tableData = new List<TableRowData>
        {
            new TableRowData { Column1 = 1, Column2 = "123456789", Column3 = "Contacto 1", Column4 = "Departamento 1"},
            new TableRowData { Column1 = 2, Column2 = "987654321", Column3 = "Contacto 2", Column4 = "Departamento 2"},
            new TableRowData { Column1 = 3, Column2 = "987654321", Column3 = "Contacto 2", Column4 = "Departamento 2"},
            new TableRowData { Column1 = 4, Column2 = "987654321", Column3 = "Contacto 2", Column4 = "Departamento 2"},
            new TableRowData { Column1 = 5, Column2 = "987654321", Column3 = "Contacto 2", Column4 = "Departamento 2"},
            new TableRowData { Column1 = 6, Column2 = "987654321", Column3 = "Contacto 2", Column4 = "Departamento 2"},
            new TableRowData { Column1 = 7, Column2 = "987654321", Column3 = "Contacto 2", Column4 = "Departamento 2"},
            new TableRowData { Column1 = 8, Column2 = "987654321", Column3 = "Contacto 2", Column4 = "Departamento 2"},
            new TableRowData { Column1 = 9, Column2 = "987654321", Column3 = "Contacto 2", Column4 = "Departamento 2"},
            // Agrega más datos aquí según sea necesario
        };

    }
}
