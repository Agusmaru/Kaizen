using Kaizen.Infrastructure.Persistence;
using Kaizen.Domain.Entities;
using Kaizen.Web.ViewModels;
using Microsoft.EntityFrameworkCore;
namespace Kaizen.Web.Services;

public class GoalHistoryService(ApplicationDbContext db)
{
    public async Task<List<GoalHistoryItemViewModel>> GetAsync(int goalId, int page = 1, int pageSize = 30, CancellationToken ct = default)
    {
        page = Math.Max(1, page); pageSize = Math.Clamp(pageSize, 1, 50); var take = page * pageSize;
        var persisted = await db.EventosHistorialMeta.AsNoTracking().Where(x => x.MetaId == goalId).OrderByDescending(x => x.FechaOcurrencia).Take(take).Select(x => new { x.FechaOcurrencia, x.Tipo, x.Descripcion, ActionName = x.AccionPlanificada != null ? x.AccionPlanificada.Nombre : null, x.ValoresAnteriores, x.ValoresNuevos, x.Motivo }).ToListAsync(ct);
        var items = persisted.Select(x => new GoalHistoryItemViewModel(x.FechaOcurrencia, Label(x.Tipo), x.Descripcion, x.ActionName, x.ValoresAnteriores, x.ValoresNuevos, x.Motivo)).ToList(); items.AddRange(await db.RegistrosAccion.AsNoTracking().Where(x => x.AccionProgramada!.AccionPlanificada!.MetaId == goalId).OrderByDescending(x => x.FechaRegistro).Take(take).Select(x => new GoalHistoryItemViewModel(x.FechaRegistro, "Registro diario", x.AccionProgramada!.AccionPlanificada!.Nombre + " — " + (x.Estado == EstadoCumplimiento.Completada ? "Realizada" : "No realizada"), x.AccionProgramada.AccionPlanificada.Nombre, null, x.ValorReal != null ? "Cantidad: " + x.ValorReal : null, x.Nota)).ToListAsync(ct));
        var goal = await db.Metas.AsNoTracking().Where(x => x.Id == goalId).Select(x => new { x.FechaCreacion, x.FechaActivacion, x.Titulo }).SingleAsync(ct); items.Add(new(goal.FechaCreacion, "Meta creada", $"Meta creada: {goal.Titulo}.", null, null, null, null)); if (goal.FechaActivacion is { } activated) items.Add(new(activated, "Meta activada", "Meta activada y planificación iniciada.", null, null, null, null)); return items.OrderByDescending(x => x.FechaOcurrencia).Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }
    private static string Label(TipoEventoHistorialMeta type) => type switch { TipoEventoHistorialMeta.Creada => "Meta creada", TipoEventoHistorialMeta.BorradorGuardado => "Borrador guardado", TipoEventoHistorialMeta.Activada => "Meta activada", TipoEventoHistorialMeta.AccionAgregada => "Acción agregada", TipoEventoHistorialMeta.Revision => "Revisión Kaizen", TipoEventoHistorialMeta.AccionMantenida => "Acción mantenida", TipoEventoHistorialMeta.AccionSimplificada => "Acción simplificada", TipoEventoHistorialMeta.AccionAumentada => "Acción aumentada", TipoEventoHistorialMeta.AccionPausada => "Acción pausada", TipoEventoHistorialMeta.AccionReanudada => "Acción reanudada", TipoEventoHistorialMeta.AccionReemplazada => "Acción reemplazada", TipoEventoHistorialMeta.AccionCompletada => "Acción completada", TipoEventoHistorialMeta.AccionModificadaManualmente => "Modificación manual de acción", TipoEventoHistorialMeta.MetaArchivada => "Meta archivada", TipoEventoHistorialMeta.AccionArchivada => "Acción archivada", _ => "Cambio de estado" };
}
