using Kaizen.Domain.Entities;
using Kaizen.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Kaizen.Tests;

public class AislamientoUsuariosTests
{
    [Fact]
    public async Task Cada_usuario_solamente_consulta_sus_datos()
    {
        var opciones = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using (var preparacion = new ApplicationDbContext(opciones))
        {
            var area = new AreaPersonal { Nombre = "Personal" };
            var metaUno = CrearMeta("usuario-1", "Meta uno", area);
            var metaDos = CrearMeta("usuario-2", "Meta dos", area);
            metaUno.Acciones.Add(new AccionPlanificada { Nombre = "Acción uno", FechaInicio = new(2026, 8, 21), VigenteDesde = new(2026, 8, 21) });
            metaDos.Acciones.Add(new AccionPlanificada { Nombre = "Acción dos", FechaInicio = new(2026, 8, 21), VigenteDesde = new(2026, 8, 21) });
            metaUno.Revisiones.Add(CrearRevision("Revisión uno"));
            metaDos.Revisiones.Add(CrearRevision("Revisión dos"));
            preparacion.AddRange(metaUno, metaDos);
            await preparacion.SaveChangesAsync();
        }

        await using var usuarioUno = new ApplicationDbContext(opciones, new UsuarioActualPrueba("usuario-1"));
        await using var usuarioDos = new ApplicationDbContext(opciones, new UsuarioActualPrueba("usuario-2"));

        Assert.Equal("Meta uno", (await usuarioUno.Metas.SingleAsync()).Titulo);
        Assert.Equal("Acción uno", (await usuarioUno.AccionesPlanificadas.SingleAsync()).Nombre);
        Assert.Equal("Revisión uno", (await usuarioUno.RevisionesKaizen.SingleAsync()).QueFunciono);
        Assert.Null(await usuarioUno.Metas.SingleOrDefaultAsync(x => x.Titulo == "Meta dos"));

        Assert.Equal("Meta dos", (await usuarioDos.Metas.SingleAsync()).Titulo);
        Assert.Equal("Acción dos", (await usuarioDos.AccionesPlanificadas.SingleAsync()).Nombre);
        Assert.Equal("Revisión dos", (await usuarioDos.RevisionesKaizen.SingleAsync()).QueFunciono);
        Assert.Null(await usuarioDos.Metas.SingleOrDefaultAsync(x => x.Titulo == "Meta uno"));
    }

    private static Meta CrearMeta(string usuarioId, string titulo, AreaPersonal area) => new()
    {
        UsuarioId = usuarioId,
        Titulo = titulo,
        Descripcion = "Descripción",
        AreaPersonal = area,
        PorQueEsImportante = "Importa",
        SituacionActual = "Actual",
        ResultadoEsperado = "Esperado",
        MetricaProgreso = "Sesiones"
    };

    private static RevisionKaizen CrearRevision(string texto) => new()
    {
        InicioPeriodo = new(2026, 8, 18),
        FinPeriodo = new(2026, 8, 24),
        QueFunciono = texto,
        QueDificulto = "Nada",
        AjustePequeno = "Continuar",
        Aprendizaje = "Aprendizaje",
        CambioProximoPeriodo = "Sin cambios",
        FechaProximaRevision = new(2026, 8, 31)
    };
}
