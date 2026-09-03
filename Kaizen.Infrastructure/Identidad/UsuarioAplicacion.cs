using Microsoft.AspNetCore.Identity;

namespace Kaizen.Infrastructure.Identidad;

public sealed class UsuarioAplicacion : IdentityUser
{
    public bool DebeCambiarClave { get; set; }
    public ICollection<SesionUsuario> Sesiones { get; set; } = [];
}
