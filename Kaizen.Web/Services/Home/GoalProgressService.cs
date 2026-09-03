using Kaizen.Infrastructure.Persistence;
using Kaizen.Domain.Entities;
using Kaizen.Web.ViewModels;
using Microsoft.EntityFrameworkCore;
namespace Kaizen.Web.Services;

public class GoalProgressService(ApplicationDbContext db)
{
    public async Task<List<GoalProgressViewModel>> EvaluateAsync(IEnumerable<Meta> goals, DateOnly? today = null)
    {
        var date = today ?? DateOnly.FromDateTime(DateTime.Today); var start = date.AddDays(-6); var result = new List<GoalProgressViewModel>();
        foreach (var goal in goals) { var rows = await db.AccionesProgramadas.Where(x => x.AccionPlanificada!.MetaId == goal.Id && x.FechaProgramada >= start && x.FechaProgramada <= date).ToListAsync(); var pct = rows.Count == 0 ? 0 : Math.Round(100m * rows.Count(x => x.Estado == EstadoCumplimiento.Completada) / rows.Count); var overdue = goal.FechaObjetivo is not null && goal.FechaObjetivo < date && goal.Estado != EstadoMeta.Completada; var state = goal.Estado switch { EstadoMeta.Borrador => GoalHealth.Draft, EstadoMeta.Pausada => GoalHealth.Paused, EstadoMeta.Completada => GoalHealth.OnTrack, _ when overdue => GoalHealth.Behind, _ when rows.Count >= 3 && pct < 50 => GoalHealth.Behind, _ when rows.Count >= 2 && pct < 80 => GoalHealth.Attention, _ => GoalHealth.OnTrack }; result.Add(new() { Meta = goal, Percentage = pct, Health = state }); }
        return result;
    }
}
