using System.ComponentModel.DataAnnotations;
using Kaizen.Domain.Entities;
namespace Kaizen.Web.ViewModels;

public class KaizenReviewFormViewModel : IValidatableObject
{
    public int MetaId { get; set; }
    public string GoalTitle { get; set; } = ""; public DateOnly InicioPeriodo { get; set; }
    public DateOnly FinPeriodo { get; set; }
    public DateOnly PlannedReviewDate { get; set; }
    public bool IsEarly { get; set; }
    public EstadoMeta EstadoMetaInicial { get; set; }
    public EstadoMeta EstadoMetaFinal { get; set; }
    public int Planned { get; set; }
    public int Completed { get; set; }
    public int Missed { get; set; }
    public int Pending { get; set; }
    public decimal PorcentajeCumplimiento => Planned == 0 ? 0 : Math.Round(100m * Completed / Planned, 2); public decimal? PreviousPercentage { get; set; }
    public string? BestAction { get; set; }
    public string? DifficultAction { get; set; }
    public string? MostMissedDays { get; set; }
    public List<string> Notes { get; set; } = []; public List<ReviewActionResultViewModel> ActionResults { get; set; } = [];
    [Required(ErrorMessage = "Contá qué funcionó bien.")] public string QueFunciono { get; set; } = ""; [Required(ErrorMessage = "Contá qué dificultades aparecieron.")] public string QueDificulto { get; set; } = ""; [Required(ErrorMessage = "Contá qué aprendiste.")] public string Aprendizaje { get; set; } = ""; [Required(ErrorMessage = "Indicá qué querés cambiar para el próximo período.")] public string CambioProximoPeriodo { get; set; } = ""; public string? NotasAdicionales { get; set; }
    public List<ReviewDecisionViewModel> Decisions { get; set; } = [];
    public IEnumerable<ValidationResult> Validate(ValidationContext context) { if (FinPeriodo < InicioPeriodo) yield return new("El período de revisión no es válido.", [nameof(InicioPeriodo)]); if (Decisions.Count == 0) yield return new("No hay acciones activas para revisar.", [nameof(Decisions)]); for (var i = 0; i < Decisions.Count; i++) { var d = Decisions[i]; if (d.FechaVigencia <= FinPeriodo) yield return new("La fecha efectiva debe ser posterior al período revisado.", [$"Decisions[{i}].FechaVigencia"]); if (d.Tipo is TipoAjuste.Simplificar or TipoAjuste.Aumentar or TipoAjuste.Reemplazar && string.IsNullOrWhiteSpace(d.Motivo)) yield return new("Explicá el motivo del ajuste.", [$"Decisions[{i}].Motivo"]); if (d.Tipo == TipoAjuste.Reemplazar && string.IsNullOrWhiteSpace(d.NewName)) yield return new("Ingresá el nombre de la acción reemplazante.", [$"Decisions[{i}].NewName"]); if (d.FechaReanudacion.HasValue && d.FechaReanudacion < d.FechaVigencia) yield return new("La reanudación no puede ser anterior a la pausa.", [$"Decisions[{i}].FechaReanudacion"]); } }
}
