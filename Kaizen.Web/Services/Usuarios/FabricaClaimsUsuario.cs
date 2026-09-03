using System.Security.Claims;
using Kaizen.Infrastructure.Identidad;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Kaizen.Web.Services;

public sealed class FabricaClaimsUsuario(
    UserManager<UsuarioAplicacion> userManager,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<UsuarioAplicacion>(userManager, optionsAccessor)
{
    public const string ClaimCambioClave = "kaizen_debe_cambiar_clave";

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(UsuarioAplicacion user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim(ClaimCambioClave, user.DebeCambiarClave ? "true" : "false"));
        return identity;
    }
}
