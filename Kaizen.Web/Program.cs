using Kaizen.Infrastructure.Persistence;
using Kaizen.Application.Abstractions;
using Kaizen.Application.DailyActions;
using Kaizen.Web.Services;
using Kaizen.Domain.Rules;
using Kaizen.Application.Usuarios;
using Kaizen.Infrastructure.Identidad;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Kaizen.Web.Filters;
var builder = WebApplication.CreateBuilder(args);
var cadenaKaizen = builder.Configuration.GetConnectionString("KaizenDb")
    ?? throw new InvalidOperationException("Falta configurar la cadena de conexión 'KaizenDb'.");
var directorioClaves = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "DataProtectionKeys");
Directory.CreateDirectory(directorioClaves);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(directorioClaves))
    .SetApplicationName("Kaizen");
builder.Services.AddControllersWithViews(options => options.Filters.Add<AntiforgeryExpiradoFilter>());
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(cadenaKaizen));
builder.Services.AddDbContext<ContextoIdentidad>(options => options.UseSqlServer(cadenaKaizen));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioActual, UsuarioActual>();
builder.Services.AddIdentity<UsuarioAplicacion, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 10;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
    .AddEntityFrameworkStores<ContextoIdentidad>()
    .AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Cuenta/Ingresar";
    options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.Events.OnValidatePrincipal = async context =>
    {
        var usuarioId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        var valorSesion = context.Principal?.FindFirstValue(ServicioSesionesUsuario.ClaimSesion);
        var valida = usuarioId is not null
            && Guid.TryParse(valorSesion, out var sesionId)
            && await context.HttpContext.RequestServices
                .GetRequiredService<ServicioSesionesUsuario>()
                .ValidarAsync(usuarioId, sesionId, context.HttpContext.RequestAborted);
        if (!valida)
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        }
    };
});
builder.Services.AddScoped<IUserClaimsPrincipalFactory<UsuarioAplicacion>, FabricaClaimsUsuario>();
builder.Services.AddScoped<InicializadorCuentaInicial>();
builder.Services.AddScoped<ServicioSesionesUsuario>();
builder.Services.AddScoped<IDailyActionRepository, DailyActionRepository>();
builder.Services.AddScoped<RegisterDailyAction>();
builder.Services.AddScoped<UndoDailyAction>();
builder.Services.AddScoped<ReorderDailyActions>();
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<GoalProgressService>();
builder.Services.AddScoped<GoalActivationService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<GoalHistoryService>();
builder.Services.AddScoped<DeletionService>();
builder.Services.AddScoped<ManualActionEditService>();
var app = builder.Build();
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection(); app.UseRouting(); app.UseAuthentication();
app.Use(async (context, next) =>
{
    var debeCambiar = context.User.FindFirst(FabricaClaimsUsuario.ClaimCambioClave)?.Value == "true";
    var rutaPermitida = context.Request.Path.StartsWithSegments("/Cuenta/CambiarClave")
        || context.Request.Path.StartsWithSegments("/Cuenta/Salir")
        || context.Request.Path.StartsWithSegments("/css")
        || context.Request.Path.StartsWithSegments("/js")
        || context.Request.Path.StartsWithSegments("/lib");
    if (debeCambiar && !rutaPermitida)
    {
        context.Response.Redirect("/Cuenta/CambiarClave");
        return;
    }
    await next();
});
app.UseAuthorization(); app.MapStaticAssets();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}").WithStaticAssets();
using (var scope = app.Services.CreateScope())
{
    var identidad = scope.ServiceProvider.GetRequiredService<ContextoIdentidad>();
    await identidad.Database.MigrateAsync();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<InicializadorCuentaInicial>().InicializarAsync();
    await DemoDataSeeder.SeedAsync(db);
}
app.Run();

public partial class Program;
