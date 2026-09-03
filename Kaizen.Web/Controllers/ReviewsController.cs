using Kaizen.Infrastructure.Persistence;
using Kaizen.Web.Services;
using Kaizen.Domain.Rules;
using Kaizen.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace Kaizen.Web.Controllers;

[Authorize]
public class ReviewsController(ApplicationDbContext db, ReviewService reviews, ILogger<ReviewsController> logger) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct) => View(await db.RevisionesKaizen.AsNoTracking().Include(x => x.Meta).OrderByDescending(x => x.FechaCreacion).ToListAsync(ct));
    public async Task<IActionResult> Create(int goalId, CancellationToken ct) { var vm = await reviews.BuildAsync(goalId, DateOnly.FromDateTime(DateTime.Today), ct); if (vm is null) { TempData["ErrorMessage"] = "No existe un período nuevo para revisar o la meta no está activa."; return RedirectToAction("Details", "Goals", new { id = goalId }); } return View(vm); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(KaizenReviewFormViewModel vm, CancellationToken ct)
    {
        var current = await reviews.BuildAsync(vm.MetaId, DateOnly.FromDateTime(DateTime.Today), ct); if (current is null) ModelState.AddModelError(string.Empty, "El período ya fue revisado o la meta no está activa."); else { current.QueFunciono = vm.QueFunciono; current.QueDificulto = vm.QueDificulto; current.Aprendizaje = vm.Aprendizaje; current.CambioProximoPeriodo = vm.CambioProximoPeriodo; current.NotasAdicionales = vm.NotasAdicionales; foreach (var posted in vm.Decisions) { var target = current.Decisions.SingleOrDefault(x => x.AccionPlanificadaId == posted.AccionPlanificadaId); if (target is null) continue; target.Tipo = posted.Tipo; target.FechaVigencia = posted.FechaVigencia; target.Motivo = posted.Motivo; target.NewName = posted.NewName; target.NewFrequency = posted.NewFrequency; target.NewWeekdays = posted.NewWeekdays; target.NewTime = posted.NewTime; target.NewTargetAmount = posted.NewTargetAmount; target.NewMetricUnit = posted.NewMetricUnit; target.NewEndDate = posted.NewEndDate; target.FechaReanudacion = posted.FechaReanudacion; } vm = current; TryValidateModel(vm); }
        if (!ModelState.IsValid) return View(vm); try { await reviews.SaveAsync(vm, ct); TempData["SuccessMessage"] = "La revisión Kaizen se guardó correctamente."; return RedirectToAction("Details", "Goals", new { id = vm.MetaId }); } catch (InvalidOperationException ex) { logger.LogWarning(ex, "Revisión rechazada para meta {MetaId}.", vm.MetaId); ModelState.AddModelError(string.Empty, ex.Message); return View(vm); } catch (Exception ex) { logger.LogError(ex, "Falló la revisión de la meta {MetaId}.", vm.MetaId); ModelState.AddModelError(string.Empty, "No se pudo guardar la revisión ni aplicar los ajustes. Intentá nuevamente."); return View(vm); }
    }
}
