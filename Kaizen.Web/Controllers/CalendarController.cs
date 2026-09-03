using Kaizen.Infrastructure.Persistence;
using Kaizen.Application.DailyActions;
using Kaizen.Domain.Entities;
using Kaizen.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Kaizen.Web.Controllers;

[Authorize]
public class CalendarController(ApplicationDbContext db, RegisterDailyAction registerDailyAction, ILogger<CalendarController> logger) : Controller
{
    [HttpGet]
    public IActionResult Index(DateOnly? month)
    {
        var initial = month ?? new DateOnly(DateTime.Today.Year, DateTime.Today.Month, 1);
        ViewBag.InitialDate = new DateOnly(initial.Year, initial.Month, 1);
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Events(DateOnly start, DateOnly end, CancellationToken ct)
    {
        if (!CalendarRange.TryCreate(start, end, out _))
            return BadRequest(new { message = $"El rango debe ser válido y no superar {CalendarRange.MaximumDays} días." });

        var timer = System.Diagnostics.Stopwatch.StartNew();
        var events = await db.AccionesProgramadas
            .AsNoTracking()
            .Where(x => x.FechaProgramada >= start && x.FechaProgramada < end && ((x.AccionPlanificada!.Meta!.Estado != EstadoMeta.Archivada && x.AccionPlanificada.Estado != EstadoAccion.Archivada) || x.Estado != EstadoCumplimiento.Pendiente || x.Registro != null))
            .OrderBy(x => x.FechaProgramada)
            .ThenBy(x => x.AccionPlanificada!.Hora)
            .Select(x => new CalendarEventDto(
                x.Id,
                x.AccionPlanificada!.Nombre,
                x.AccionPlanificada.Meta!.Titulo,
                x.FechaProgramada,
                x.AccionPlanificada.Hora,
                x.Estado.ToString(),
                x.AccionPlanificada.MetaId,
                "/Goals/Details/" + x.AccionPlanificada.MetaId,
                x.Registro != null ? x.Registro.Nota : null))
            .ToListAsync(ct);

        logger.LogInformation("Calendario: rango {Start} a {End}, {Count} eventos, {ElapsedMs} ms.", start, end, events.Count, timer.ElapsedMilliseconds);
        return Json(events);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(int id, EstadoCumplimiento status, string? note, decimal? actualValue, CancellationToken ct)
    {
        var result = await registerDailyAction.ExecuteAsync(id, status, actualValue, note, null, ct);
        if (!result.Success) return BadRequest(new { message = result.Message });

        if (Request.GetTypedHeaders().Accept?.Any(x => x.MediaType.Value == "application/json") == true)
            return Json(new
            {
                message = result.Message,
                status = result.Estado.ToString(),
                statusLabel = result.Estado == EstadoCumplimiento.Completada ? "Realizada" : "No realizada",
                actualValue = result.ValorReal,
                note = result.Nota
            });

        var scheduledDate = await db.AccionesProgramadas.Where(x => x.Id == id).Select(x => x.FechaProgramada).SingleAsync(ct);
        return RedirectToAction(nameof(Index), new { month = scheduledDate });
    }
}
