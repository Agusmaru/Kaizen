using Kaizen.Infrastructure.Identidad;
using Microsoft.AspNetCore.Identity;

namespace Kaizen.Web.Services;

public sealed class InicializadorCuentaInicial(
    UserManager<UsuarioAplicacion> userManager,
    IConfiguration configuration,
    ILogger<InicializadorCuentaInicial> logger)
{
    public async Task InicializarAsync()
    {
        var usuario = await userManager.FindByIdAsync(CuentaInicial.UsuarioId)
            ?? throw new InvalidOperationException("No se encontró la cuenta inicial de Kaizen.");

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
