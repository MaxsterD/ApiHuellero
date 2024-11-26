using ConsolaBlazor.Components.Modales;
using ConsolaBlazor.CustomStyle;
using ConsolaBlazor.Services.DTOs.AsignarEmpleados;
using ConsolaBlazor.Services.DTOs.Horarios;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using static MudBlazor.CategoryTypes;

namespace ConsolaBlazor.Pages.AdministrarTiempos
{
    public partial class AdministrarTiempos
    {
        private DateRange _dateRange;
        private EmpleadosDTO Lider = new EmpleadosDTO();
        private EmpleadosDTO Empleado = new EmpleadosDTO();
        [Inject] HttpClient httpClient { get; set; }
        [Inject] AuthenticationStateProvider autenticacionProvider { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IConfiguration Configuration { get; set; }

        private string titleBarStyle = $"height:7%;background-color:{AtowerTheme.Default.PaletteLight.Primary}; color:white;";
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
            var dialog = await DialogService.ShowAsync<BuscarLupa<EmpleadosDTO>>("", parametros, options);
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

        private async Task DeleteServerAsync(TableRowDataDate datos)
        {
            var parameters = new DialogParameters<ConfirmActionModalDate> { { x => x.Server, datos } };

            var dialog = await DialogService.ShowAsync<ConfirmActionModalDate>("Borrar Registro", parameters);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                //In a real world scenario we would reload the data from the source here since we "removed" it in the dialog already.

                Console.WriteLine(result.Data.ToString());

                tableData.RemoveAll(item => item.Column1 == (int)result.Data);
            }
        }

      

        private void Buscar()
        {

            tableData = tableData2
            .Where(row =>
            {
                // Intenta convertir Column3 a DateTime
                if (DateTime.TryParse(row.Column3, out DateTime column3Date))
                {
                    // Compara con los rangos de fecha
                    return column3Date >= _dateRange.Start.Value &&
                           column3Date <= _dateRange.End.Value.AddDays(1).AddTicks(-1);
                }
                return false; // Excluye filas con formato inválido
            })
            .ToList();


        }

        private Func<TableRowDataDate, int, string> _rowStyleFunc => (x, i) =>
        {
            if (x.Column5 == null)
                return "background-color:#ff8b8b8f";

            return "";
        };

        private List<TableRowDataDate> tableData = new List<TableRowDataDate>
        {
            new TableRowDataDate { Column1 = 1, Column2 = "123456789", Column3 ="1/11/2024",Column4 = new DateTime(2024, 11, 1, 18, 0, 0), Column5 = new DateTime(2024, 11, 1, 10, 33, 0), Column6 = "Departamento 2"},
            new TableRowDataDate { Column1 = 2, Column2 = "987654321", Column3 ="2/11/2024",Column4 = new DateTime(2024, 11, 2, 17, 0, 0), Column5 = new DateTime(2024, 11, 2, 9, 0, 0), Column6 = "Departamento 2"},
            new TableRowDataDate { Column1 = 3, Column2 = "987654321", Column3 ="2/11/2024", Column4 = new DateTime(2024, 11, 2, 9, 0, 0), Column5 = null, Column6 = "Departamento 2"},
            new TableRowDataDate { Column1 = 4, Column2 = "987654321", Column3 ="3/11/2024",Column4 = new DateTime(2024, 11, 3, 19, 0, 0), Column5 = null, Column6 = "Departamento 2"},
            new TableRowDataDate { Column1 = 5, Column2 = "987654321", Column3 ="3/11/2024",Column4 = new DateTime(2024, 11, 3, 19, 0, 0), Column5 = new DateTime(2024, 11, 2, 9, 0, 0), Column6 = "Departamento 2"},
            new TableRowDataDate { Column1 = 6, Column2 = "987654321", Column3 ="2/11/2024",Column4 = new DateTime(2024, 11, 2, 17, 0, 0), Column5 = new DateTime(2024, 11, 2, 9, 0, 0), Column6 = "Departamento 2"},
            new TableRowDataDate { Column1 = 7, Column2 = "987654321", Column3 ="3/11/2024",Column4 = new DateTime(2024, 11, 3, 11, 0, 0), Column5 = new DateTime(2024, 11, 2, 9, 0, 0), Column6 = "Departamento 2"},
            new TableRowDataDate { Column1 = 8, Column2 = "987654321", Column3 ="3/11/2024",Column4 = new DateTime(2024, 11, 3, 11, 0, 0), Column5 = new DateTime(2024, 11, 2, 17, 0, 0), Column6 = "Departamento 2"},
            new TableRowDataDate { Column1 = 9, Column2 = "987654321", Column3 ="3/11/2024",Column4 = new DateTime(2024, 11, 3, 19, 0, 0), Column5 = new DateTime(2024, 11, 2, 17, 0, 0), Column6 = "Departamento 2"},
        };

        private List<TableRowDataDate> tableData2 = new List<TableRowDataDate>
        {
            new TableRowDataDate { Column1 = 1, Column2 = "123456789", Column3 ="1/11/2024",Column4 = new DateTime(2024, 11, 1, 18, 0, 0), Column5 = new DateTime(2024, 11, 1, 10, 33, 0), Column6 = "Departamento 2"},
            new TableRowDataDate { Column1 = 2, Column2 = "987654321", Column3 ="2/11/2024",Column4 = new DateTime(2024, 11, 2, 17, 0, 0), Column5 = new DateTime(2024, 11, 2, 9, 0, 0), Column6 = "Departamento 2"},
            new TableRowDataDate { Column1 = 3, Column2 = "987654321", Column3 ="2/11/2024", Column4 = new DateTime(2024, 11, 2, 9, 0, 0), Column5 = null, Column6 = "Departamento 2"},
            new TableRowDataDate { Column1 = 4, Column2 = "987654321", Column3 ="3/11/2024",Column4 = new DateTime(2024, 11, 3, 19, 0, 0), Column5 = null, Column6 = "Departamento 2"},
            new TableRowDataDate { Column1 = 5, Column2 = "987654321", Column3 ="3/11/2024",Column4 = new DateTime(2024, 11, 3, 19, 0, 0), Column5 = new DateTime(2024, 11, 2, 9, 0, 0), Column6 = "Departamento 2"},
            new TableRowDataDate { Column1 = 6, Column2 = "987654321", Column3 ="2/11/2024",Column4 = new DateTime(2024, 11, 2, 17, 0, 0), Column5 = new DateTime(2024, 11, 2, 9, 0, 0), Column6 = "Departamento 2"},
            new TableRowDataDate { Column1 = 7, Column2 = "987654321", Column3 ="3/11/2024",Column4 = new DateTime(2024, 11, 3, 11, 0, 0), Column5 = new DateTime(2024, 11, 2, 9, 0, 0), Column6 = "Departamento 2"},
            new TableRowDataDate { Column1 = 8, Column2 = "987654321", Column3 ="3/11/2024",Column4 = new DateTime(2024, 11, 3, 11, 0, 0), Column5 = new DateTime(2024, 11, 2, 17, 0, 0), Column6 = "Departamento 2"},
            new TableRowDataDate { Column1 = 9, Column2 = "987654321", Column3 ="3/11/2024",Column4 = new DateTime(2024, 11, 3, 19, 0, 0), Column5 = new DateTime(2024, 11, 2, 17, 0, 0), Column6 = "Departamento 2"},
        };
    }
}
