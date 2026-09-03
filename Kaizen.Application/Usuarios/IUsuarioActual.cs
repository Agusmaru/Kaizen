namespace Kaizen.Application.Usuarios;

public interface IUsuarioActual
{
    string? UsuarioId { get; }
    bool EstaAutenticado { get; }
}
