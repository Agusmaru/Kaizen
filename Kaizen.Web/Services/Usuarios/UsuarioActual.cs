using System.Security.Claims;
using Kaizen.Application.Usuarios;

namespace Kaizen.Web.Services;

public sealed class UsuarioActual(IHttpContextAccessor httpContextAccessor) : IUsuarioActual
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public string? UsuarioId => Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

    public bool EstaAutenticado => Principal?.Identity?.IsAuthenticated == true && UsuarioId is not null;
}
