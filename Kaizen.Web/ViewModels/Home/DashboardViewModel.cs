using Kaizen.Domain.Entities;

namespace Kaizen.Web.ViewModels;

public class DashboardViewModel { public List<TodayActionViewModel> Today { get; set; } = []; public List<GoalProgressViewModel> Metas { get; set; } = []; public List<GoalProgressViewModel> GoalsRequiringAttention => Metas.Where(x => x.Health is GoalHealth.Attention or GoalHealth.Behind).ToList(); public List<Meta> UpcomingReviews { get; set; } = []; public List<Meta> DueReviews => UpcomingReviews.Where(x => x.FechaProximaRevision <= DateOnly.FromDateTime(DateTime.Today)).ToList(); public decimal WeeklyPercentage { get; set; } public int ResolvedToday => Today.Count(x => x.Estado != EstadoCumplimiento.Pendiente); }
