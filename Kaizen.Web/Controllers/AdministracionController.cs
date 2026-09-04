using Kaizen.Infrastructure.Identidad;
using Kaizen.Web.Services;
using Kaizen.Web.ViewModels.Administracion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kaizen.Web.Controllers;

[Authorize(Roles = InicializadorCuentaInicial.RolAdministrador)]
public sealed class AdministracionController(
    UserManager<UsuarioAplicacion> userManager,
    ServicioSesionesUsuario sesiones) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Usuarios()
    {
        var usuarios = await userManager.Users
            .OrderBy(x => x.Email)
            .ToListAsync();
        var resultado = new List<UsuarioAdministracionViewModel>();

        foreach (var usuario in usuarios)
        {
            resultado.Add(new UsuarioAdministracionViewModel
            {
                Id = usuario.Id,
                Correo = usuario.Email ?? usuario.UserName ?? "Sin correo",
                EsAdministrador = await userManager.IsInRoleAsync(usuario, InicializadorCuentaInicial.RolAdministrador),
                DebeCambiarClave = usuario.DebeCambiarClave,
                EstaBloqueado = usuario.LockoutEnd > DateTimeOffset.UtcNow
            });
        }

        return View(resultado);
    }

    [HttpGet]
    public async Task<IActionResult> RestablecerClave(string id)
    {
        var usuario = await userManager.FindByIdAsync(id);
        if (usuario is null) return NotFound();

        return View(new RestablecerClaveViewModel
        {
            UsuarioId = usuario.Id,
            Correo = usuario.Email ?? usuario.UserName ?? "Sin correo"
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RestablecerClave(RestablecerClaveViewModel vm)
    {
        var usuario = await userManager.FindByIdAsync(vm.UsuarioId);
        if (usuario is null) return NotFound();
        vm.Correo = usuario.Email ?? usuario.UserName ?? "Sin correo";
        if (!ModelState.IsValid) return View(vm);

        var token = await userManager.GeneratePasswordResetTokenAsync(usuario);
        var resultado = await userManager.ResetPasswordAsync(usuario, token, vm.ClaveTemporal);
        if (!resultado.Succeeded)
        {
            foreach (var error in resultado.Errors)
                ModelState.AddModelError(nameof(vm.ClaveTemporal), TraducirError(error.Code));
            return View(vm);
        }

        usuario.DebeCambiarClave = true;
        await userManager.UpdateAsync(usuario);
        await userManager.UpdateSecurityStampAsync(usuario);
        await sesiones.RevocarTodasAsync(usuario.Id);

        TempData["SuccessMessage"] = $"Se asignó una contraseña temporal a {vm.Correo}.";
        return RedirectToAction(nameof(Usuarios));
    }

    private static string TraducirError(string codigo) => codigo switch
    {
        "PasswordTooShort" => "La contraseña es demasiado corta.",
        "PasswordRequiresNonAlphanumeric" => "Debe incluir al menos un símbolo.",
        "PasswordRequiresDigit" => "Debe incluir al menos un número.",
        "PasswordRequiresUpper" => "Debe incluir al menos una mayúscula.",
        "PasswordRequiresLower" => "Debe incluir al menos una minúscula.",
        _ => "No se pudo restablecer la contraseña."
    };
}
