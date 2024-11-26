using Blazored.SessionStorage;
using ConsolaBlazor;
using ConsolaBlazor.CustomStyle;
using ConsolaBlazor.Services.Http;
using ConsolaBlazor.Services.Interfaces;
using ConsolaBlazor.Services.SessionStore;
using ConsolaBlazor.Services.SessionStore.StoreSession;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMudBlazorDialog();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = false;
    config.SnackbarConfiguration.VisibleStateDuration = 1000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});


builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddSingleton<AtowerTheme>();
builder.Services.AddBlazoredSessionStorage();

builder.Services.AddScoped<IHttpService,HttpService>();
builder.Services.AddScoped<ClienteSession>();
builder.Services.AddScoped<AuthenticationStateProvider, AutenticacionExtension>();
builder.Services.AddAuthorizationCore();


await builder.Build().RunAsync();
