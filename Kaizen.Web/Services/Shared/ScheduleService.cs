using Kaizen.Infrastructure.Persistence;
using Kaizen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Kaizen.Web.Services;

public class ScheduleService(ApplicationDbContext db)
{
    public const int MaximumHorizonDays = 90;
    public Task EnsureScheduledAsync(DateOnly from, DateOnly to, CancellationToken ct = default) => EnsureScheduledAsync(null, from, to, ct);
    public Task EnsureGoalScheduledAsync(int goalId, DateOnly from, DateOnly to, CancellationToken ct = default) => EnsureScheduledAsync(goalId, from, to, ct);
    private async Task EnsureScheduledAsync(int? goalId, DateOnly from, DateOnly to, CancellationToken ct)
    {
        if (to < from) throw new ArgumentException("La fecha final no puede ser anterior a la inicial.", nameof(to));
        var safeTo = to > from.AddDays(MaximumHorizonDays) ? from.AddDays(MaximumHorizonDays) : to;
        var query = db.AccionesPlanificadas.Where(x => x.Estado == EstadoAccion.Activa && x.Meta!.Estado == EstadoMeta.Activa);
        if (goalId.HasValue) query = query.Where(x => x.MetaId == goalId.Value);
        var items = await query.ToListAsync(ct);
        var itemIds = items.Select(x => x.Id).ToList();
        var keys = (await db.AccionesProgramadas.Where(x => itemIds.Contains(x.AccionPlanificadaId) && x.FechaProgramada >= from && x.FechaProgramada <= safeTo).Select(x => new { x.AccionPlanificadaId, x.FechaProgramada }).ToListAsync(ct)).Select(x => $"{x.AccionPlanificadaId}:{x.FechaProgramada}").ToHashSet();
        foreach (var item in items) for (var date = from; date <= safeTo; date = date.AddDays(1)) if (KaizenRules.OccursOn(item, date) && keys.Add($"{item.Id}:{date}")) db.AccionesProgramadas.Add(new() { AccionPlanificadaId = item.Id, FechaProgramada = date });
        await db.SaveChangesAsync(ct);
    }
}
