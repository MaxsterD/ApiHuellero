using ConsolaBlazor.Components.Modales;
using ConsolaBlazor.CustomStyle;
using ConsolaBlazor.Services.DTOs.Horarios;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ConsolaBlazor.Services.DTOs.Login;
using ConsolaBlazor.Pages.Login;
using System.Net.Http.Json;
using ConsolaBlazor.Services.DTOs.AsignarEmpleados;

namespace ConsolaBlazor.Pages.AsignacionLider
{
    public partial class AsignacionLider
    {
        private EmpleadosDTO Lider = new EmpleadosDTO();
        private EmpleadosDTO Empleado = new EmpleadosDTO();
        [Inject] HttpClient httpClient { get; set; }
        [Inject] AuthenticationStateProvider autenticacionProvider { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IConfiguration Configuration { get; set; }

        private string titleBarStyle = $"max-width:2000px;height:7%;background-color:{AtowerTheme.Default.PaletteLight.Primary}; color:white;";
        private string headerBarStyle = $"background-color:{AtowerTheme.Default.PaletteLight.Primary}; color:white;";

        private EmpleadosDTO Criterio = new EmpleadosDTO();

        private async Task BuscarLideres()
        {
           
            var UrlGet = $"api/AsignarLideres/ListarEmpleados";
            var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, BackdropClick = false };
            var parametros = new DialogParameters<BuscarLupa<EmpleadosDTO>>
            {
                {x => x.Url, UrlGet},
                {x => x.OpcionesType, new CriterioEmpleadoDTO()},
                {x => x.Titulo, "Buscar Lideres"}
            };
            var dialog = await DialogService.ShowAsync <BuscarLupa<EmpleadosDTO>>("", parametros, options);
            var result = await dialog.Result;
            

            if (result != null && !result.Canceled && result.Data != null)
            {
                Criterio = result.Data as EmpleadosDTO;
                Lider.Id = Criterio.Id;
                Lider.Nombre = Criterio.Nombre;
                Lider.Identificacion = Criterio.Identificacion;
                
            }
        }

        private async Task BuscarEmpleados()
        {

            var UrlGet = $"api/AsignarLideres/ListarEmpleados";
            var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, BackdropClick = false };
            var parametros = new DialogParameters<BuscarLupa<EmpleadosDTO>>
            {
                {x => x.Url, UrlGet},
                {x => x.OpcionesType, new CriterioEmpleadoDTO()},
                {x => x.Titulo, "Buscar Empleados"}
            };
            var dialog = await DialogService.ShowAsync<BuscarLupa<EmpleadosDTO>>("", parametros, options);
            var result = await dialog.Result;


            if (result != null && !result.Canceled && result.Data != null)
            {
                Criterio = result.Data as EmpleadosDTO;
                Empleado.Id = Criterio.Id;
                Empleado.Nombre = Criterio.Nombre;
                Empleado.Identificacion = Criterio.Identificacion;
            }
        }

        private async Task DeleteServerAsync(TableRowData datos)
        {
            var parameters = new DialogParameters<ConfirmActionModal> { { x => x.Server, datos } };

            var dialog = await DialogService.ShowAsync<ConfirmActionModal>("Borrar Registro", parameters);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                //In a real world scenario we would reload the data from the source here since we "removed" it in the dialog already.

                Console.WriteLine(result.Data.ToString());

                tableData.RemoveAll(item => item.Column1 == (int)result.Data);
            }
        }

        private void Guardar()
        {
            if (Lider == null)
            {
                Snackbar.Add("Por favor, introduce los datos del Lider.", Severity.Warning, config => { config.HideIcon = true; });
                return;
            }else if (Empleado == null)
            {
                Snackbar.Add("Por favor, introduce los datos del Empleado.", Severity.Warning, config => { config.HideIcon = true; });
                return;
            }
            else if (string.IsNullOrEmpty(Lider.Nombre))
            {
                Snackbar.Add("Por favor, Selecciona un Lider.", Severity.Warning, config => { config.HideIcon = true; });
                return;
            }
            else if (string.IsNullOrEmpty(Empleado.Nombre))
            {
                Snackbar.Add("Por favor, Selecciona un Empleado.", Severity.Warning, config => { config.HideIcon = true; });
                return;
            }
            else if (Lider.Id == Empleado.Id)
            {
                Snackbar.Add("Un lider no puede ser su propio empleado", Severity.Warning, config => { config.HideIcon = true; });
                return;
            }

            tableData.Add(new TableRowData { Column1 = Lider.Id, Column2 = Lider.Nombre, Column3 = Lider.Identificacion, Column4 = Empleado.Nombre, Column5 = Empleado.Identificacion });
            
            Lider.Nombre = "";
            Lider.Identificacion = "";
            Lider.Id = 0;
            Empleado.Nombre = "";
            Empleado.Identificacion = "";
            Empleado.Id = 0;


        }

        private List<TableRowData> tableData = new List<TableRowData>
        {
            new TableRowData { Column1 = 1, Column2 = "123456789", Column3 = "Contacto 1", Column4 = "Departamento 1", Column5 = "Departamento 2"},
            new TableRowData { Column1 = 2, Column2 = "987654321", Column3 = "Contacto 2", Column4 = "Departamento 2", Column5 = "Departamento 2"},
            new TableRowData { Column1 = 3, Column2 = "987654321", Column3 = "Contacto 2", Column4 = "Departamento 2", Column5 = "Departamento 2"},
            new TableRowData { Column1 = 4, Column2 = "987654321", Column3 = "Contacto 2", Column4 = "Departamento 2", Column5 = "Departamento 2"},
            new TableRowData { Column1 = 5, Column2 = "987654321", Column3 = "Contacto 2", Column4 = "Departamento 2", Column5 = "Departamento 2"},
            new TableRowData { Column1 = 6, Column2 = "987654321", Column3 = "Contacto 2", Column4 = "Departamento 2", Column5 = "Departamento 2"},
            new TableRowData { Column1 = 7, Column2 = "987654321", Column3 = "Contacto 2", Column4 = "Departamento 2", Column5 = "Departamento 2"},
            new TableRowData { Column1 = 8, Column2 = "987654321", Column3 = "Contacto 2", Column4 = "Departamento 2", Column5 = "Departamento 2"},
            new TableRowData { Column1 = 9, Column2 = "987654321", Column3 = "Contacto 2", Column4 = "Departamento 2", Column5 = "Departamento 2"},
        };
    }
}
