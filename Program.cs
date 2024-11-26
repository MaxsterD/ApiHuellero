using ApiConsola.Infrastructura.Data;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using ApiConsola.Interfaces;
using ApiConsola.Services;
using ApiConsola.Services.ConsultarTicket;
using ApiConsola.Services.Http;
using ApiConsola.Services.Interfaces;
using ApiConsola.Services.CrearTicket;
using ApiConsola.Interfaces.AsignarLideres;
using ApiConsola.Services.AsignarLideres;
using ApiConsola.Interfaces.CreacionUsuario;
using ApiConsola.Services.CreacionUsuario;

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

builder.Services.AddScoped<ISqlServerDbContext, SqlServerDbContext>();
builder.Services.AddScoped<ICrearTicketService, CrearTicketService>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IAsignarLideresService, AsignarLideresService>();
builder.Services.AddScoped<ICreacionUsuarioService, CreacionUsuarioService>();
builder.Services.AddScoped<ITicketService, ConsultarTicketService>();
builder.Services.AddScoped<ITicketByClienteService, ConsultarByClienteService>();
builder.Services.AddScoped<ICrearComentarioService, CrearComentarioService>();
builder.Services.AddScoped<ITicketStatusService, TicketStatusService>();
builder.Services.AddScoped<ISeedService, SeedService>();
builder.Services.AddScoped<IEnviarHttp, EnviarHttp>();

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

app.Run();