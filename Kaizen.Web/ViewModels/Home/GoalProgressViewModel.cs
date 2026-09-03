using Kaizen.Domain.Entities;

namespace Kaizen.Web.ViewModels;

public class GoalProgressViewModel { public required Meta Meta { get; set; } public decimal Percentage { get; set; } public GoalHealth Health { get; set; } public string Label => Health switch { GoalHealth.OnTrack => "En buen camino", GoalHealth.Attention => "Requiere atención", GoalHealth.Behind => "Con dificultades", GoalHealth.Draft => "Borrador", _ => "Pausada" }; public string BootstrapColor => Health switch { GoalHealth.OnTrack => "success", GoalHealth.Attention => "warning", GoalHealth.Behind => "danger", _ => "secondary" }; }
