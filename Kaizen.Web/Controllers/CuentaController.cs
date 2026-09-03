using Kaizen.Infrastructure.Identidad;
using Kaizen.Web.Services;
using Kaizen.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kaizen.Web.Controllers;

public sealed class CuentaController(
    UserManager<UsuarioAplicacion> userManager,
    SignInManager<UsuarioAplicacion> signInManager,
    ServicioSesionesUsuario sesiones) : Controller
{
    [AllowAnonymous, HttpGet]
    public IActionResult Ingresar(string? retorno = null, bool sesionExpirada = false)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        ViewBag.SesionExpirada = sesionExpirada;
        return View(new LoginViewModel { Retorno = retorno });
    }

    [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Ingresar(LoginViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var usuario = await userManager.FindByEmailAsync(vm.Correo.Trim());
        var result = usuario is null
            ? Microsoft.AspNetCore.Identity.SignInResult.Failed
            : await signInManager.CheckPasswordSignInAsync(usuario, vm.Clave, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "El correo o la contraseña no son correctos.");
            return View(vm);
        }

        await IniciarSesionAsync(usuario!, vm.Recordarme);
        if (usuario?.DebeCambiarClave == true)
            return RedirectToAction(nameof(CambiarClave));

        return Url.IsLocalUrl(vm.Retorno)
            ? LocalRedirect(vm.Retorno!)
            : RedirectToAction("Index", "Home");
    }

    [AllowAnonymous, HttpGet]
    public IActionResult Registrarse() =>
        User.Identity?.IsAuthenticated == true
            ? RedirectToAction("Index", "Home")
            : View(new RegistroViewModel());

    [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrarse(RegistroViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var correo = vm.Correo.Trim();
        var usuario = new UsuarioAplicacion
        {
            UserName = correo,
            Email = correo,
            EmailConfirmed = true,
            DebeCambiarClave = false
        };

        var result = await userManager.CreateAsync(usuario, vm.Clave);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, TraducirError(error.Code));
            return View(vm);
        }

        await IniciarSesionAsync(usuario, recordar: false);
        return RedirectToAction("Index", "Home");
    }

    [Authorize, HttpGet]
    public IActionResult CambiarClave() => View(new CambiarClaveViewModel());

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarClave(CambiarClaveViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var usuario = await userManager.GetUserAsync(User);
        if (usuario is null) return Challenge();

        var result = await userManager.ChangePasswordAsync(usuario, vm.ClaveActual, vm.NuevaClave);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "No se pudo cambiar la contraseña. Verificá la contraseña actual y los requisitos de seguridad.");
            return View(vm);
        }

        usuario.DebeCambiarClave = false;
        await userManager.UpdateAsync(usuario);
        if (ObtenerSesionId() is { } sesionId)
        {
            await sesiones.RevocarOtrasAsync(usuario.Id, sesionId);
            await signInManager.SignOutAsync();
            await signInManager.SignInWithClaimsAsync(
                usuario,
                isPersistent: false,
                [new Claim(ServicioSesionesUsuario.ClaimSesion, sesionId.ToString())]);
        }
        TempData["SuccessMessage"] = "La contraseña se cambió correctamente.";
        return RedirectToAction("Index", "Home");
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Salir()
    {
        var usuarioId = userManager.GetUserId(User);
        if (usuarioId is not null && ObtenerSesionId() is { } sesionId)
            await sesiones.RevocarAsync(usuarioId, sesionId);
        await signInManager.SignOutAsync();
        return RedirectToAction(nameof(Ingresar));
    }

    [AllowAnonymous]
    public IActionResult AccesoDenegado() => View();

    private async Task IniciarSesionAsync(UsuarioAplicacion usuario, bool recordar)
    {
        var sesion = await sesiones.CrearAsync(
            usuario.Id,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            Request.Headers.UserAgent.ToString());
        await signInManager.SignInWithClaimsAsync(
            usuario,
            recordar,
            [new Claim(ServicioSesionesUsuario.ClaimSesion, sesion.Id.ToString())]);
    }

    private Guid? ObtenerSesionId() =>
        Guid.TryParse(User.FindFirstValue(ServicioSesionesUsuario.ClaimSesion), out var id) ? id : null;

    private static string TraducirError(string code) => code switch
    {
        "DuplicateUserName" or "DuplicateEmail" => "Ya existe una cuenta con ese correo.",
        "PasswordTooShort" => "La contraseña es demasiado corta.",
        "PasswordRequiresNonAlphanumeric" => "La contraseña debe incluir al menos un símbolo.",
        "PasswordRequiresDigit" => "La contraseña debe incluir al menos un número.",
        "PasswordRequiresUpper" => "La contraseña debe incluir al menos una mayúscula.",
        "PasswordRequiresLower" => "La contraseña debe incluir al menos una minúscula.",
        _ => "No se pudo crear la cuenta. Revisá los datos ingresados."
    };
}
