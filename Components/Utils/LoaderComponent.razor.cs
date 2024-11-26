using Microsoft.AspNetCore.Components;

namespace ConsolaBlazor.Components.Utils
{
    public partial class LoaderComponent
    {
        [Parameter] public bool Loading { get; set; } = false;
    }
}
