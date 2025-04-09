using MercadoPago.Config;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProgressusWebApi.DataContext;
using ProgressusWebApi.Repositories.EjercicioRepositories;
using ProgressusWebApi.Repositories.EjercicioRepositories.Interfaces;
using ProgressusWebApi.Repositories.Interfaces;
using ProgressusWebApi.Repositories.MembresiaRepositories;
using ProgressusWebApi.Repositories.MembresiaRepositories.Interfaces;
using ProgressusWebApi.Repositories.NotificacionesRepositories;
using ProgressusWebApi.Repositories.NotificacionesRepositories.Interfaces;
using ProgressusWebApi.Repositories.PlanEntrenamientoRepositories;
using ProgressusWebApi.Repositories.PlanEntrenamientoRepositories.Interfaces;
using ProgressusWebApi.Repositories.QrCodeRepositories;
using ProgressusWebApi.Services.AlimentoServices;
using ProgressusWebApi.Services.AuthServices;
using ProgressusWebApi.Services.AuthServices.Interfaces;
using ProgressusWebApi.Services.CarritoServices;
using ProgressusWebApi.Services.EjercicioServices;
using ProgressusWebApi.Services.EjercicioServices.Interfaces;
using ProgressusWebApi.Services.EmailServices;
using ProgressusWebApi.Services.EmailServices.Interfaces;
using ProgressusWebApi.Services.InventarioServices;
using ProgressusWebApi.Services.InventarioServices.Interfaces;
using ProgressusWebApi.Services.MembresiaServices;
using ProgressusWebApi.Services.MembresiaServices.Interfaces;
using ProgressusWebApi.Services.MerchServices;
using ProgressusWebApi.Services.MerchServices.Interfaces;
using ProgressusWebApi.Services.NotificacionesServices;
using ProgressusWebApi.Services.NotificacionesServices.Interfaces;
using ProgressusWebApi.Services.PedidosServices;
using ProgressusWebApi.Services.PedidosServices.Interfaces;
using ProgressusWebApi.Services.PlanEntrenamientoServices;
using ProgressusWebApi.Services.PlanEntrenamientoServices.Interfaces;
using ProgressusWebApi.Services.QrCodeServices;
using ProgressusWebApi.Services.ReservaService;
using ProgressusWebApi.Services.ReservaService.cs.interfaces;
using ProgressusWebApi.Services.ReservaServices;
using ProgressusWebApi.Utilities;
using Swashbuckle.AspNetCore.Filters;
using WebApiMercadoPago.Repositories;
using WebApiMercadoPago.Repositories.Interface;
using WebApiMercadoPago.Services;
using WebApiMercadoPago.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de CORS para aceptar peticiones de cualquier origen
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAllOrigins",
        policy =>
        {
            policy
                .AllowAnyOrigin() // Permite cualquier origen
                .AllowAnyHeader() // Permite cualquier encabezado
                .AllowAnyMethod(); // Permite cualquier m�todo (GET, POST, etc.)
        }
    );
});

// Agregar los servicios al contenedor
// Configuraci�n para ignorar referencias c�clicas en la serializaci�n JSON
builder
    .Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft
            .Json
            .ReferenceLoopHandling
            .Ignore;
    });

// Inyecci�n de repositorios y servicios
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEjercicioRepository, EjercicioRepository>();
builder.Services.AddScoped<IEjercicioService, EjercicioService>();
builder.Services.AddScoped<IMusculoDeEjercicioRepository, MusculoDeEjercicioRepository>();
builder.Services.AddScoped<IMusculoDeEjercicioService, MusculoDeEjercicioService>();
builder.Services.AddScoped<IMusculoRepository, MusculoRepository>();
builder.Services.AddScoped<IMusculoService, MusculoService>();
builder.Services.AddScoped<IGrupoMuscularRepository, GrupoMuscularRepository>();
builder.Services.AddScoped<IGrupoMuscularService, GrupoMuscularService>();
builder.Services.AddScoped<IPlanDeEntrenamientoRepository, PlanDeEntrenamientoRepository>();
builder.Services.AddScoped<IPlanDeEntrenamientoService, PlanDeEntrenamientoService>();
builder.Services.AddScoped<IDiaDePlanRepository, DiaDePlanRepository>();
builder.Services.AddScoped<IEjercicioEnDiaDelPlanRepository, EjercicioEnDiaDelPlanRepository>();
builder.Services.AddScoped<IObjetivoDelPlanRepository, ObjetivoDelPlanRepository>();
builder.Services.AddScoped<ISerieDeEjercicioRepository, SerieDeEjercicioRepository>();
builder.Services.AddScoped<IObjetivoDelPlanService, ObjetivoDelPlanService>();
builder.Services.AddScoped<IAsignacionDePlanRepository, AsignacionDePlanRepository>();
builder.Services.AddScoped<IAsignacionDePlanService, AsignacionDePlanService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IMembresiaRepository, MembresiaRepository>();
builder.Services.AddScoped<IMembresiaService, MembresiaService>();
builder.Services.AddScoped<ISolicitudDePagoRepository, SolicitudDePagoRepository>();
builder.Services.AddScoped<ISolicitudDePagoService, SolicitudDePagoService>();
builder.Services.AddScoped<AlimentoCalculoService>();
builder.Services.AddScoped<QrCodeRepository>();
builder.Services.AddScoped<QrCodeService>();

builder.Services.AddMemoryCache();

// Permitir documentaci�n y acceso de los endpoints con swagger
// Configuraci�n con oauth2 para requerir autorizaci�n en la ejecuci�n de los endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "oauth2",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
        }
    );
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

// Registrar IHttpClientFactory
builder.Services.AddHttpClient();

// Conexi�n a la base de datos
builder.Services.AddDbContext<ProgressusDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Autenticaci�n y autorizaci�n con Identity (endpoints)
builder.Services.AddAuthorization();
builder
    .Services.AddIdentityApiEndpoints<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ProgressusDataContext>();

// Configuraci�n para envio de emails con servidor SMTP de Gmail
builder.Services.AddTransient<IEmailSenderService, EmailSenderService>();
builder.Services.Configure<GmailSetter>(builder.Configuration.GetSection("GmailSettings"));

// Configuraci�n para sistema de cobros con MercadoPago

builder.Services.AddScoped<IMercadoPagoRepository, MercadoPagoRepository>();
builder.Services.AddScoped<IMercadoPagoService, MercadoPagoService>();
MercadoPagoConfig.AccessToken =
    "APP_USR-2278733141716614-062815-583c9779901a7bbf32c8e8a73971e44c-1878150528";
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IInventarioService, InventarioService>();
builder.Services.AddScoped<INotificacionesService, NotificacionesService>();
builder.Services.AddScoped<IMerchService, MerchService>();

// Registrar servicios
builder.Services.AddScoped<ICarritoService, CarritoService>();

//Notificaciones services - repository
builder.Services.AddScoped<INotificacionesUsuariosService, NotificacionesUsuariosService>();
builder.Services.AddScoped<IEstadosNotificacionesService, EstadosNotificacionesService>();
builder.Services.AddScoped<IPlantillasService, PlantillasService>();
builder.Services.AddScoped<ITiposNotificacionesService, TiposNotificacionesService>();

builder.Services.AddScoped<IEstadoNotificacionRepository, EstadoNotificacionRepository>();
builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();
builder.Services.AddScoped<IPlantillaRepository, PlantillaRepository>();
builder.Services.AddScoped<ITipoNotificacionRepository, TipoNotificacionRepository>();
builder.Services.AddScoped<IAuthService, AuthService>(); //  Correcto

// Construir la aplicaci�n con todas las configuraciones y servicios definidos en el objeto builder
var app = builder.Build();

// Configuraci�n de la pipeline del HTTP request
// Comentamos esta linea para poder deployar en produccion
//if (app.Environment.IsDevelopment())
//{
//app.UseSwagger();
//app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

// Aplicar CORS para permitir cualquier origen
app.UseCors("AllowAllOrigins");

// Mapear endpoints de authorization y authentication
app.MapIdentityApi<IdentityUser>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
