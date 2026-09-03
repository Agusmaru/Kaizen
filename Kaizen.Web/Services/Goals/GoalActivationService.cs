using Kaizen.Infrastructure.Persistence;
using Kaizen.Domain.Entities;
using Kaizen.Web.ViewModels;
using Kaizen.Application.Usuarios;
using Microsoft.EntityFrameworkCore;
namespace Kaizen.Web.Services;

public class GoalActivationService(ApplicationDbContext db, ScheduleService schedule, IUsuarioActual? usuarioActual = null)
{
    public async Task<int> SaveAsync(GoalSetupViewModel vm, bool activate, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct); var isNew = vm.Id == 0; var goal = isNew ? new Meta { UsuarioId = usuarioActual?.UsuarioId ?? throw new InvalidOperationException("No se pudo identificar al usuario actual.") } : await db.Metas.Include(x => x.Evaluacion).Include(x => x.Acciones).SingleAsync(x => x.Id == vm.Id, ct); var priorStatus = goal.Estado; if (isNew) { db.Metas.Add(goal); goal.EventosHistorial.Add(new() { Tipo = TipoEventoHistorialMeta.Creada, Descripcion = "Meta creada." }); }
        goal.Titulo = vm.Titulo!.Trim(); goal.Descripcion = vm.Descripcion?.Trim() ?? ""; goal.AreaPersonalId = vm.AreaPersonalId; goal.FechaInicio = vm.FechaInicio; goal.FechaObjetivo = vm.FechaObjetivo; goal.MetricaProgreso = vm.MetricaProgreso?.Trim() ?? ""; goal.PorQueEsImportante = vm.PorQueEsImportante?.Trim() ?? ""; goal.SituacionActual = vm.SituacionActual?.Trim() ?? ""; goal.ResultadoEsperado = vm.ResultadoEsperado?.Trim() ?? ""; var assessment = goal.Evaluacion ?? new EvaluacionKaizen(); assessment.DondeEstoy = goal.SituacionActual; assessment.QueCambiar = goal.ResultadoEsperado; assessment.Obstaculos = vm.Obstaculos?.Trim() ?? ""; assessment.QueFunciona = vm.QueFunciona?.Trim() ?? ""; assessment.MejoraMasPequena = vm.MejoraMasPequena?.Trim() ?? ""; assessment.EvidenciaMejora = vm.EvidenciaMejora?.Trim() ?? ""; assessment.DificultadPercibida = vm.DificultadPercibida; if (goal.Evaluacion is null) goal.Evaluacion = assessment;
        if (goal.Acciones.Count > 0) db.AccionesPlanificadas.RemoveRange(goal.Acciones); goal.Acciones = []; foreach (var item in vm.Acciones.Where(x => !x.IsEmpty)) { var start = item.FechaInicio ?? vm.FechaInicio; var action = new AccionPlanificada { Nombre = item.Nombre!.Trim(), Frecuencia = item.Frecuencia, DiasSemana = item.DiasSemana, Hora = item.Hora, FechaInicio = start, VigenteDesde = start, FechaFin = item.FechaFin, CantidadObjetivo = item.CantidadObjetivo, UnidadMetrica = item.UnidadMetrica?.Trim(), DificultadEstimada = item.DificultadEstimada, Estado = EstadoAccion.Activa }; goal.Acciones.Add(action); goal.EventosHistorial.Add(new() { Tipo = TipoEventoHistorialMeta.AccionAgregada, AccionPlanificada = action, Descripcion = $"Acción agregada: {action.Nombre}." }); }
        goal.Estado = activate ? EstadoMeta.Activa : EstadoMeta.Borrador; goal.FechaProximaRevision = activate ? DateOnly.FromDateTime(DateTime.Today).AddDays(7) : null; if (activate && priorStatus != EstadoMeta.Activa) { goal.FechaActivacion = DateTime.UtcNow; goal.EventosHistorial.Add(new() { Tipo = TipoEventoHistorialMeta.Activada, Descripcion = "Meta activada." }); } else if (!activate) goal.EventosHistorial.Add(new() { Tipo = TipoEventoHistorialMeta.BorradorGuardado, Descripcion = "Meta guardada como borrador." }); await db.SaveChangesAsync(ct);
        if (activate) { var today = DateOnly.FromDateTime(DateTime.Today); var horizon = goal.FechaObjetivo is { } target && target < today.AddDays(ScheduleService.MaximumHorizonDays) ? target : today.AddDays(ScheduleService.MaximumHorizonDays); if (horizon >= today) await schedule.EnsureGoalScheduledAsync(goal.Id, today, horizon, ct); }
        await tx.CommitAsync(ct); return goal.Id;
    }
}
