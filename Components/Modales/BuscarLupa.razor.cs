using ConsolaBlazor.Services.DTOs;
using ConsolaBlazor.Services.Http;
using ConsolaBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;

namespace ConsolaBlazor.Components.Modales
{
    public partial class BuscarLupa<T> : ComponentBase
    {
        [Inject] private HttpClient HttpClient { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }
        [Inject] private IConfiguration Configuration { get; set; }
        [Inject] private IHttpService httpGet { get; set; }

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [Parameter] public string Url { get; set; } = "";
        [Parameter] public string Titulo { get; set; } = "Buscar Lupa";
        [Parameter] public object? OpcionesType { get; set; } = null;
        [Parameter] public List<string> ColumnasXParametro { get; set; } = new List<string>();


        private List<string> OpcionesBusqueda { get; set; } = new List<string>();
        private string TextoBusqueda { get; set; } = "";
        private string SelectedParametro { get; set; } = "";
        private List<string> ColumnasTabla { get; set; } = new List<string>();
        private List<T> DatosTabla { get; set; } = new List<T>();
        private bool Loading = false;
        private T? selectedRow;

        protected override async Task OnInitializedAsync()
        {
            if(OpcionesType != null)
            {
                OpcionesBusqueda = GetColumnasTabla<T>(OpcionesType);
                SelectedParametro = OpcionesBusqueda[0];
            }
            else
            {
                OpcionesBusqueda = new List<string>();
            }
            ColumnasTabla = ColumnasXParametro.Count > 0 ? ColumnasXParametro : GetColumnasTabla<T>();
            var result = await httpGet.FetchData<T>(Url);

            if (result != null)
            {
                DatosTabla = result;
            }
        }

        private async Task SearchFn(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                if (string.IsNullOrEmpty(SelectedParametro))
                {
                    Snackbar.Add("Elija un parámetro de búsqueda", Severity.Warning, config => { config.HideIcon = true; });
                    return;
                }
                if (!string.IsNullOrEmpty(TextoBusqueda))
                {
                    string newUrl = $"{Url}?{SelectedParametro}={TextoBusqueda}";
                    var result = await httpGet.FetchData<T>(newUrl);
                    if (result != null)
                    {
                        DatosTabla = result;
                    }
                }
                else
                {
                    var result = await httpGet.FetchData<T>(Url);
                    if (result != null)
                    {
                        DatosTabla = result;
                    }
                }
            }
        }

        private void SelectRowFn()
        {
            if (selectedRow == null)
            {
                Snackbar.Add("Elija un parámetro de búsqueda", Severity.Warning, config => { config.HideIcon = true; });
            }
            else
            {
                MudDialog.Close(DialogResult.Ok(selectedRow));
            }
        }

        private void CloseFn()
        {
            MudDialog?.Close(DialogResult.Cancel());
        }

        private object GetPropertyValue(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            return property != null ? property.GetValue(obj) : null;
        }

        private List<string> GetColumnasTabla<B>(object? obj = null)
        {
            Type DtoType;
            if (obj != null)
            {
                DtoType = obj.GetType();
            }
            else
            {
                DtoType = typeof(B);
            }
            PropertyInfo[] propiedades = DtoType.GetProperties();

            var columnasTabla = new List<string>();
            foreach (PropertyInfo pi in propiedades)
            {
                columnasTabla.Add(pi.Name);
            }

            return columnasTabla;
        }

    }
}
