using Kaizen.Infrastructure.Identidad;
using Microsoft.AspNetCore.Identity;

namespace Kaizen.Web.Services;

public sealed class InicializadorCuentaInicial(
    UserManager<UsuarioAplicacion> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration,
    ILogger<InicializadorCuentaInicial> logger)
{
    public const string RolAdministrador = "Administrador";

    public async Task InicializarAsync()
    {
        var usuario = await userManager.FindByIdAsync(CuentaInicial.UsuarioId)
            ?? throw new InvalidOperationException("No se encontró la cuenta inicial de Kaizen.");

        if (!await roleManager.RoleExistsAsync(RolAdministrador))
        {
            var resultadoRol = await roleManager.CreateAsync(new IdentityRole(RolAdministrador));
            if (!resultadoRol.Succeeded)
                throw new InvalidOperationException(string.Join(" ", resultadoRol.Errors.Select(x => x.Description)));
        }

        if (!await userManager.IsInRoleAsync(usuario, RolAdministrador))
        {
            var resultadoAsignacion = await userManager.AddToRoleAsync(usuario, RolAdministrador);
            if (!resultadoAsignacion.Succeeded)
                throw new InvalidOperationException(string.Join(" ", resultadoAsignacion.Errors.Select(x => x.Description)));
        }

        if (await userManager.HasPasswordAsync(usuario)) return;

        var claveTemporal = configuration["CuentaInicial:ClaveTemporal"];
        if (string.IsNullOrWhiteSpace(claveTemporal))
            throw new InvalidOperationException("Configurá CuentaInicial:ClaveTemporal mediante secretos o variables de entorno antes de iniciar Kaizen.");

        var result = await userManager.AddPasswordAsync(usuario, claveTemporal);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(" ", result.Errors.Select(x => x.Description)));

        logger.LogInformation("La cuenta inicial {Correo} quedó preparada y requiere cambio de contraseña.", CuentaInicial.Email);
    }
}
