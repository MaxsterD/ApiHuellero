using ApiConsola.Infrastructura.Data;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using ApiConsola.Interfaces;
using ApiConsola.Services;
using ApiConsola.Services.Http;
using ApiConsola.Services.Interfaces;
using ApiConsola.Interfaces.AsignarLideres;
using ApiConsola.Services.AsignarLideres;
using ApiConsola.Interfaces.CreacionUsuario;
using ApiConsola.Services.CreacionUsuario;
using ApiConsola.Interfaces.Horarios;
using ApiConsola.Services.Horarios;
using ApiConsola.Services.DTOs.InfoDevice;
using ApiConsola.Services.ConexionHuellero;
using ApiConsola.Interfaces.ConexionHuellero;
using ApiConsola;
using Microsoft.Data.SqlClient;
using System.Data;
using ApiConsola.Interfaces.AsignarHorario;
using ApiConsola.Services.AsignarHorario;
using ApiConsola.Interfaces.Parametros;
using ApiConsola.Services.Parametros;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSwaggerUI",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api Consola", Version = "v1" });

    // Configuración para la autenticación con JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});


builder.Services.AddDbContext<SqlServerDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Consola"));
});



//builder.Services.AddHostedService<ActualizarFechaBackground>(); // Aquí es donde lo registras


builder.Services.AddScoped<ISqlServerDbContext, SqlServerDbContext>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IAsignarLideresService, AsignarLideresService>();
builder.Services.AddScoped<ICreacionUsuarioService, CreacionUsuarioService>();
builder.Services.AddScoped<IHorariosService, HorariosService>();
builder.Services.AddSingleton<IInfoDevice, InfoDeviceDTO>();
builder.Services.AddScoped<IHuelleroService, HuelleroService>();
builder.Services.AddScoped<IEnviarHttp, EnviarHttp>();
builder.Services.AddScoped<IAsignarHorarioService, AsignarHorarioService>();
builder.Services.AddScoped<IParametrosService, ParametrosService>();

var jwtKey = builder.Configuration["JWT:key"];
var encryptionKey = builder.Configuration["JWT:encryptionKey"];
if (jwtKey != null && encryptionKey != null)
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptionKey)) // Agrega la clave de cifrado
            };
        });
}
else
{
    throw new InvalidOperationException("La configuración del token de seguridad JWT:key o JWT:encryptionKey no está especificada en el archivo de configuración.");
}


var app = builder.Build();


app.UseCors("AllowSwaggerUI");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Consola");
    c.OAuthClientId("swagger");
    c.OAuthClientSecret("secret");
    c.OAuthRealm("realm");
    c.OAuthAppName("Mi API");
});

app.Use(async (context, next) =>
{
    try
    {
        await next();

        if (context.Response.StatusCode == 401)
        {
            var result = JsonSerializer.Serialize(new { Success = false, Mensaje = "No se ha autenticado para realizar este proceso." });
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(result);

        }
        else if (context.Response.StatusCode == 403)
        {
            var result = JsonSerializer.Serialize(new { Success = false, Mensaje = "No tienes permiso para realizar esta acción." });
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(result);
        }
        else if (context.Response.StatusCode == 422)
        {
            var result = JsonSerializer.Serialize(new { Success = false, Mensaje = "Debes ingresar datos válidos." });
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(result);
        }
        else if (context.Response.StatusCode == 500)
        {
            var result = JsonSerializer.Serialize(new { Success = false, Mensaje = "Ha ocurrido un error en el servidor." });
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(result);
        }
    }
    catch (Exception ex)
    {
        var errorMessage = ex.Message;
        var result = JsonSerializer.Serialize(new { Success = false, Mensaje = errorMessage });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync(result);
    }
});


app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


//using (var scope = app.Services.CreateScope())
//{
//    var conexionHuelleroService = scope.ServiceProvider.GetRequiredService<IHuelleroService>();
//    await conexionHuelleroService.ConectarDispositivo();
//    conexionHuelleroService.IniciarSincronizacionPeriodica(60);
//}


app.Run();