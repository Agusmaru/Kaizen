using System.ComponentModel.DataAnnotations;
using Kaizen.Domain.Entities;

namespace Kaizen.Web.ViewModels;

public class GoalSetupActionViewModel
{
    public string? Nombre { get; set; }
    public FrecuenciaAccion Frecuencia { get; set; } = FrecuenciaAccion.Diaria; public string? DiasSemana { get; set; }
    public TimeOnly? Hora { get; set; }
    public DateOnly? FechaInicio { get; set; } = DateOnly.FromDateTime(DateTime.Today); public DateOnly? FechaFin { get; set; }
    public decimal? CantidadObjetivo { get; set; }
    public string? UnidadMetrica { get; set; }
    public int DificultadEstimada { get; set; } = 1;
    public bool IsEmpty => string.IsNullOrWhiteSpace(Nombre) && !CantidadObjetivo.HasValue && string.IsNullOrWhiteSpace(DiasSemana);
    public IEnumerable<ValidationResult> Validate(int index) { if (IsEmpty) yield break; if (string.IsNullOrWhiteSpace(Nombre)) yield return new("Ingresá el nombre de la acción.", [$"Acciones[{index}].Nombre"]); if (!FechaInicio.HasValue) yield return new("Ingresá la fecha inicial.", [$"Acciones[{index}].FechaInicio"]); if (FechaFin.HasValue && FechaInicio.HasValue && FechaFin < FechaInicio) yield return new("La fecha final no puede ser anterior a la inicial.", [$"Acciones[{index}].FechaFin"]); if (Frecuencia == FrecuenciaAccion.DiasSeleccionados && string.IsNullOrWhiteSpace(DiasSemana)) yield return new("Seleccioná los días correspondientes.", [$"Acciones[{index}].DiasSemana"]); if (CantidadObjetivo < 0) yield return new("La cantidad no puede ser negativa.", [$"Acciones[{index}].CantidadObjetivo"]); }
}
