using Kaizen.Infrastructure.Identidad;
using Microsoft.EntityFrameworkCore;

namespace Kaizen.Web.Services;

public sealed class ServicioSesionesUsuario(ContextoIdentidad contexto)
{
    public const string ClaimSesion = "kaizen_sesion_id";
    public const int MaximoSesiones = 2;
    private static readonly TimeSpan Duracion = TimeSpan.FromHours(8);
    private static readonly TimeSpan IntervaloActualizacion = TimeSpan.FromMinutes(5);

    public async Task<SesionUsuario> CrearAsync(
        string usuarioId,
        string? direccionIp,
        string? dispositivo,
        CancellationToken cancellationToken = default)
    {
        var ahora = DateTime.UtcNow;
        var activas = await contexto.SesionesUsuario
            .Where(x => x.UsuarioId == usuarioId && x.FechaRevocacion == null && x.FechaVencimiento > ahora)
            .OrderBy(x => x.UltimaActividad)
            .ToListAsync(cancellationToken);

        foreach (var anterior in activas.Take(Math.Max(0, activas.Count - MaximoSesiones + 1)))
            anterior.FechaRevocacion = ahora;

        var sesion = new SesionUsuario
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            FechaInicio = ahora,
            UltimaActividad = ahora,
            FechaVencimiento = ahora.Add(Duracion),
            DireccionIp = Limitar(direccionIp, 45),
            Dispositivo = Limitar(dispositivo, 250)
        };
        contexto.SesionesUsuario.Add(sesion);
        await contexto.SaveChangesAsync(cancellationToken);
        return sesion;
    }

    public async Task<bool> ValidarAsync(string usuarioId, Guid sesionId, CancellationToken cancellationToken = default)
    {
        var ahora = DateTime.UtcNow;
        var sesion = await contexto.SesionesUsuario.SingleOrDefaultAsync(
            x => x.Id == sesionId && x.UsuarioId == usuarioId,
            cancellationToken);

        if (sesion is null || sesion.FechaRevocacion is not null || sesion.FechaVencimiento <= ahora)
            return false;

        if (ahora - sesion.UltimaActividad >= IntervaloActualizacion)
        {
            sesion.UltimaActividad = ahora;
            sesion.FechaVencimiento = ahora.Add(Duracion);
            await contexto.SaveChangesAsync(cancellationToken);
        }
        return true;
    }

    public async Task RevocarAsync(string usuarioId, Guid sesionId, CancellationToken cancellationToken = default)
    {
        var sesion = await contexto.SesionesUsuario.SingleOrDefaultAsync(
            x => x.Id == sesionId && x.UsuarioId == usuarioId && x.FechaRevocacion == null,
            cancellationToken);
        if (sesion is null) return;
        sesion.FechaRevocacion = DateTime.UtcNow;
        await contexto.SaveChangesAsync(cancellationToken);
    }

    public async Task RevocarOtrasAsync(string usuarioId, Guid sesionActualId, CancellationToken cancellationToken = default)
    {
        var ahora = DateTime.UtcNow;
        var otras = await contexto.SesionesUsuario
            .Where(x => x.UsuarioId == usuarioId && x.Id != sesionActualId && x.FechaRevocacion == null)
            .ToListAsync(cancellationToken);
        foreach (var sesion in otras) sesion.FechaRevocacion = ahora;
        await contexto.SaveChangesAsync(cancellationToken);
    }

    public async Task RevocarTodasAsync(string usuarioId, CancellationToken cancellationToken = default)
    {
        var ahora = DateTime.UtcNow;
        var sesiones = await contexto.SesionesUsuario
            .Where(x => x.UsuarioId == usuarioId && x.FechaRevocacion == null)
            .ToListAsync(cancellationToken);
        foreach (var sesion in sesiones) sesion.FechaRevocacion = ahora;
        await contexto.SaveChangesAsync(cancellationToken);
    }

    private static string? Limitar(string? valor, int longitud) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor[..Math.Min(valor.Length, longitud)];
}
