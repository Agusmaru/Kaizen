using Kaizen.Application.DailyActions;
using Kaizen.Infrastructure.Persistence;
using Kaizen.Domain.Entities;
using Kaizen.Web.Services;
using Kaizen.Domain.Rules;
using Kaizen.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace Kaizen.Web.Controllers;

[Authorize]
public class HomeController(ApplicationDbContext db, GoalProgressService progress, RegisterDailyAction registerDailyAction, UndoDailyAction undoDailyAction, ILogger<HomeController> logger) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var t = DateOnly.FromDateTime(DateTime.Today); var active = db.AccionesProgramadas.AsNoTracking().Where(x => x.AccionPlanificada!.Meta!.Estado == EstadoMeta.Activa && x.AccionPlanificada.Estado == EstadoAccion.Activa);
        var goals = await db.Metas.AsNoTracking().Include(x => x.AreaPersonal).Where(x => x.Estado == EstadoMeta.Activa).ToListAsync(ct);
        var rows = await active.Where(x => x.FechaProgramada >= t.AddDays(-6) && x.FechaProgramada <= t).ToListAsync(ct);
        var today = await active.Where(x => x.FechaProgramada == t).OrderBy(x => x.Estado).ThenBy(x => x.AccionPlanificada!.Hora).Select(x => new TodayActionViewModel { Id = x.Id, Nombre = x.AccionPlanificada!.Nombre, MetaId = x.AccionPlanificada.MetaId, GoalTitle = x.AccionPlanificada.Meta!.Titulo, Estado = x.Estado, CantidadObjetivo = x.AccionPlanificada.CantidadObjetivo, UnidadMetrica = x.AccionPlanificada.UnidadMetrica, Hora = x.AccionPlanificada.Hora, ValorReal = x.Registro != null ? x.Registro.ValorReal : null, Nota = x.Registro != null ? x.Registro.Nota : null }).ToListAsync(ct);
        var vm = new DashboardViewModel { Metas = await progress.EvaluateAsync(goals, t), Today = today, UpcomingReviews = await db.Metas.AsNoTracking().Where(x => x.Estado == EstadoMeta.Activa && x.FechaProximaRevision != null).OrderBy(x => x.FechaProximaRevision).Take(4).ToListAsync(ct), WeeklyPercentage = rows.Count == 0 ? 0 : Math.Round(100m * rows.Count(x => x.Estado == EstadoCumplimiento.Completada) / rows.Count) }; return View(vm);
    }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Registro(int id, EstadoCumplimiento status, decimal? actualValue, string? note, CancellationToken ct) { try { var result = await registerDailyAction.ExecuteAsync(id, status, actualValue, note, DateOnly.FromDateTime(DateTime.Today), ct); if (!result.Success) return BadRequest(new { message = result.Message }); return Json(new { message = result.Message, status = result.Estado.ToString(), statusLabel = result.Estado == EstadoCumplimiento.Completada ? "Realizada" : "No realizada", actualValue = result.ValorReal, note = result.Nota }); } catch (Exception ex) { logger.LogError(ex, "No se pudo registrar la acción diaria {AccionProgramadaId}.", id); return StatusCode(500, new { message = "No se pudo registrar la acción. Intentá nuevamente." }); } }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> UndoLog(int id, CancellationToken ct) { try { var result = await undoDailyAction.ExecuteAsync(id, DateOnly.FromDateTime(DateTime.Today), ct); if (!result.Success) return BadRequest(new { message = result.Message }); return Json(new { message = result.Message, status = "Pending", statusLabel = "Pendiente" }); } catch (Exception ex) { logger.LogError(ex, "No se pudo deshacer el registro diario {AccionProgramadaId}.", id); return StatusCode(500, new { message = "No se pudo deshacer el registro. Intentá nuevamente." }); } }
}
