using Kaizen.Infrastructure.Persistence;
using Kaizen.Domain.Entities;
using Kaizen.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Kaizen.Web.Services;

public enum DeletionOutcome { Deleted, Archived, AlreadyArchived, NotFound }
public sealed record DeletionResult(DeletionOutcome Outcome, int? MetaId = null);

public sealed class DeletionService(ApplicationDbContext db, ILogger<DeletionService> logger)
{
    public async Task<DeleteConfirmationViewModel?> InspectGoalAsync(int id, CancellationToken ct = default)
    {
        var goal = await db.Metas.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct); if (goal is null) return null;
        var ids = db.AccionesPlanificadas.Where(x => x.MetaId == id).Select(x => x.Id);
        var occurrences = await db.AccionesProgramadas.CountAsync(x => ids.Contains(x.AccionPlanificadaId), ct);
        var logs = await db.RegistrosAccion.CountAsync(x => ids.Contains(x.AccionProgramada!.AccionPlanificadaId), ct);
        var reviews = await db.RevisionesKaizen.CountAsync(x => x.MetaId == id, ct);
        return new() { Id = id, Titulo = goal.Titulo, Acciones = await ids.CountAsync(ct), Occurrences = occurrences, Logs = logs, Revisiones = reviews, WillArchive = !CanDeleteGoal(goal, occurrences, reviews) };
    }

    public async Task<DeleteConfirmationViewModel?> InspectActionAsync(int id, CancellationToken ct = default)
    {
        var action = await db.AccionesPlanificadas.AsNoTracking().Include(x => x.Meta).SingleOrDefaultAsync(x => x.Id == id, ct); if (action is null) return null;
        var ids = db.AccionesPlanificadas.Where(x => x.SerieAccionId == action.SerieAccionId).Select(x => x.Id);
        var occurrences = await db.AccionesProgramadas.CountAsync(x => ids.Contains(x.AccionPlanificadaId), ct);
        var logs = await db.RegistrosAccion.CountAsync(x => ids.Contains(x.AccionProgramada!.AccionPlanificadaId), ct);
        var adjustments = await db.AjustesPlan.CountAsync(x => (x.AccionPlanificadaId != null && ids.Contains(x.AccionPlanificadaId.Value)) || (x.NuevaAccionPlanificadaId != null && ids.Contains(x.NuevaAccionPlanificadaId.Value)), ct);
        return new() { Id = id, MetaId = action.MetaId, Titulo = action.Nombre, Acciones = await ids.CountAsync(ct), Occurrences = occurrences, Logs = logs, Revisiones = adjustments, WillArchive = action.Meta!.Estado != EstadoMeta.Borrador || occurrences > 0 || adjustments > 0 };
    }

    public async Task<DeletionResult> DeleteGoalAsync(int id, string? reason, CancellationToken ct = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        try
        {
            var goal = await db.Metas.Include(x => x.Acciones).SingleOrDefaultAsync(x => x.Id == id, ct); if (goal is null) return new(DeletionOutcome.NotFound);
            if (goal.Estado == EstadoMeta.Archivada) return new(DeletionOutcome.AlreadyArchived, id);
            var ids = goal.Acciones.Select(x => x.Id).ToList(); var occurrences = await db.AccionesProgramadas.CountAsync(x => ids.Contains(x.AccionPlanificadaId), ct); var reviews = await db.RevisionesKaizen.CountAsync(x => x.MetaId == id, ct);
            if (CanDeleteGoal(goal, occurrences, reviews))
            {
                var benign = await db.EventosHistorialMeta.Where(x => x.MetaId == id && (x.Tipo == TipoEventoHistorialMeta.Creada || x.Tipo == TipoEventoHistorialMeta.BorradorGuardado || x.Tipo == TipoEventoHistorialMeta.AccionAgregada)).ToListAsync(ct); db.EventosHistorialMeta.RemoveRange(benign); db.Metas.Remove(goal); await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); return new(DeletionOutcome.Deleted);
            }
            var today = DateOnly.FromDateTime(DateTime.Today); var pending = await db.AccionesProgramadas.Where(x => ids.Contains(x.AccionPlanificadaId) && x.FechaProgramada >= today && x.Estado == EstadoCumplimiento.Pendiente && x.Registro == null).ToListAsync(ct); db.AccionesProgramadas.RemoveRange(pending);
            goal.Estado = EstadoMeta.Archivada; goal.FechaArchivo = DateTime.UtcNow; goal.MotivoArchivo = Clean(reason); goal.FechaProximaRevision = null;
            foreach (var action in goal.Acciones.Where(x => x.Estado != EstadoAccion.Archivada)) { action.Estado = EstadoAccion.Archivada; action.FechaArchivo = goal.FechaArchivo; action.MotivoArchivo = goal.MotivoArchivo; action.VigenteHasta = today; }
            db.EventosHistorialMeta.Add(new() { MetaId = id, Tipo = TipoEventoHistorialMeta.MetaArchivada, Descripcion = "Meta archivada.", Motivo = goal.MotivoArchivo }); await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); return new(DeletionOutcome.Archived, id);
        }
        catch (Exception ex) { logger.LogError(ex, "No se pudo eliminar o archivar la meta {MetaId}.", id); await tx.RollbackAsync(CancellationToken.None); throw; }
    }

    public async Task<DeletionResult> DeleteActionAsync(int id, string? reason, CancellationToken ct = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        try
        {
            var selected = await db.AccionesPlanificadas.Include(x => x.Meta).SingleOrDefaultAsync(x => x.Id == id, ct); if (selected is null) return new(DeletionOutcome.NotFound);
            var versions = await db.AccionesPlanificadas.Where(x => x.SerieAccionId == selected.SerieAccionId).ToListAsync(ct); if (versions.All(x => x.Estado == EstadoAccion.Archivada)) return new(DeletionOutcome.AlreadyArchived, selected.MetaId);
            var ids = versions.Select(x => x.Id).ToList(); var occurrences = await db.AccionesProgramadas.CountAsync(x => ids.Contains(x.AccionPlanificadaId), ct); var adjustments = await db.AjustesPlan.CountAsync(x => (x.AccionPlanificadaId != null && ids.Contains(x.AccionPlanificadaId.Value)) || (x.NuevaAccionPlanificadaId != null && ids.Contains(x.NuevaAccionPlanificadaId.Value)), ct);
            if (selected.Meta!.Estado == EstadoMeta.Borrador && occurrences == 0 && adjustments == 0)
            {
                var benign = await db.EventosHistorialMeta.Where(x => x.AccionPlanificadaId != null && ids.Contains(x.AccionPlanificadaId.Value) && x.Tipo == TipoEventoHistorialMeta.AccionAgregada).ToListAsync(ct); db.EventosHistorialMeta.RemoveRange(benign); db.AccionesPlanificadas.RemoveRange(versions); await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); return new(DeletionOutcome.Deleted, selected.MetaId);
            }
            var today = DateOnly.FromDateTime(DateTime.Today); var pending = await db.AccionesProgramadas.Where(x => ids.Contains(x.AccionPlanificadaId) && x.FechaProgramada >= today && x.Estado == EstadoCumplimiento.Pendiente && x.Registro == null).ToListAsync(ct); db.AccionesProgramadas.RemoveRange(pending);
            var archivedAt = DateTime.UtcNow; foreach (var item in versions.Where(x => x.Estado != EstadoAccion.Archivada)) { item.Estado = EstadoAccion.Archivada; item.FechaArchivo = archivedAt; item.MotivoArchivo = Clean(reason); item.VigenteHasta = today; }
            db.EventosHistorialMeta.Add(new() { MetaId = selected.MetaId, AccionPlanificadaId = selected.Id, Tipo = TipoEventoHistorialMeta.AccionArchivada, Descripcion = $"Acción archivada: {selected.Nombre}.", Motivo = Clean(reason) }); await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); return new(DeletionOutcome.Archived, selected.MetaId);
        }
        catch (Exception ex) { logger.LogError(ex, "No se pudo eliminar o archivar la acción {ActionId}.", id); await tx.RollbackAsync(CancellationToken.None); throw; }
    }
    private static bool CanDeleteGoal(Meta goal, int occurrences, int reviews) => goal.Estado == EstadoMeta.Borrador && goal.FechaActivacion is null && occurrences == 0 && reviews == 0;
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
