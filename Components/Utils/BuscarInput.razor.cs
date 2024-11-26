using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualBasic;
using MudBlazor;
using System.Runtime.CompilerServices;
using ConsolaBlazor.Services.Interfaces;



namespace ConsolaBlazor.Components.Utils
{
    public partial class BuscarInput<Type>
    {
        [Inject] IHttpService HttpService { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }


        private string _codigo;
        [Parameter]
        public string Value
        {
            get => _codigo;
            set
            {
                if (_codigo != value)
                {
                    _codigo = value;
                    OnValueChanged.InvokeAsync(value);
                }
            }
        }
        [Parameter] public EventCallback<Type> OnDataFetched { get; set; }
        [Parameter] public EventCallback<string> OnValueChanged { get; set; }
        [Parameter] public string Label { get; set; } = "Código";
        [Parameter] public string Class { get; set; } = string.Empty;
        [Parameter] public string Style { get; set; } = string.Empty;
        [Parameter] public string Path { get; set; } = "";
        [Parameter] public Margin CustomMargin { get; set; } = Margin.Normal;
        [Parameter] public Variant CustomVariant { get; set; } = Variant.Text;
        [Parameter] public bool ReadOnly { get; set; } = false;

        private bool Loading = false;

        private async Task HandleOnKey(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await BuscarInputMethod();
            }
        }
        private async Task BuscarInputMethod()
        {
            if (string.IsNullOrEmpty(Value))
            {
                Snackbar.Add("El campo Código es necesario", Severity.Warning, config => { config.HideIcon = true; });
                return;
            }

            var url = $"{Path}?Codigo={Value}";
            var data = await HttpService.FetchData<Type>(url);
            if (data != null && data.Any())
            {
                var objectResult = data.FirstOrDefault();

                await OnDataFetched.InvokeAsync(objectResult);
                StateHasChanged();
            }
            else
            {
                Snackbar.Add("No existen datos con este Código", Severity.Warning, config => { config.HideIcon = true; });
            }
        }
    }
}
