using Kaizen.Infrastructure.Identidad;
using Kaizen.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace Kaizen.Tests;

public class ServicioSesionesUsuarioTests
{
    [Fact]
    public async Task La_tercera_sesion_revoca_la_mas_antigua()
    {
        await using var db = CrearContexto();
        var usuario = new UsuarioAplicacion { Id = "usuario-1", UserName = "usuario@kaizen.local" };
        db.Users.Add(usuario);
        await db.SaveChangesAsync();
        var servicio = new ServicioSesionesUsuario(db);

        var primera = await servicio.CrearAsync(usuario.Id, "127.0.0.1", "Equipo uno");
        primera.UltimaActividad = DateTime.UtcNow.AddMinutes(-2);
        await db.SaveChangesAsync();
        var segunda = await servicio.CrearAsync(usuario.Id, "127.0.0.2", "Equipo dos");
        var tercera = await servicio.CrearAsync(usuario.Id, "127.0.0.3", "Equipo tres");

        Assert.NotNull((await db.SesionesUsuario.FindAsync(primera.Id))!.FechaRevocacion);
        Assert.True(await servicio.ValidarAsync(usuario.Id, segunda.Id));
        Assert.True(await servicio.ValidarAsync(usuario.Id, tercera.Id));
        Assert.False(await servicio.ValidarAsync(usuario.Id, primera.Id));
        Assert.Equal(2, await db.SesionesUsuario.CountAsync(x => x.FechaRevocacion == null));
    }

    [Fact]
    public async Task Cerrar_sesion_la_invalida_inmediatamente()
    {
        await using var db = CrearContexto();
        var usuario = new UsuarioAplicacion { Id = "usuario-2", UserName = "otro@kaizen.local" };
        db.Users.Add(usuario);
        await db.SaveChangesAsync();
        var servicio = new ServicioSesionesUsuario(db);
        var sesion = await servicio.CrearAsync(usuario.Id, null, null);

        await servicio.RevocarAsync(usuario.Id, sesion.Id);

        Assert.False(await servicio.ValidarAsync(usuario.Id, sesion.Id));
    }

    private static ContextoIdentidad CrearContexto()
    {
        var opciones = new DbContextOptionsBuilder<ContextoIdentidad>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ContextoIdentidad(opciones);
    }
}
