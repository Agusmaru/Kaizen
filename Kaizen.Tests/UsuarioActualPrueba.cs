using Kaizen.Application.Usuarios;

namespace Kaizen.Tests;

public sealed class UsuarioActualPrueba(string usuarioId) : IUsuarioActual
{
    public string? UsuarioId { get; } = usuarioId;
    public bool EstaAutenticado => true;
}
