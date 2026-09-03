using System.ComponentModel.DataAnnotations;
using Kaizen.Domain.Entities;

namespace Kaizen.Web.ViewModels;

public class GoalSetupViewModel : IValidatableObject
{
    public int Id { get; set; }
    public string SubmitIntent { get; set; } = "activate"; public string? Titulo { get; set; }
    public string? Descripcion { get; set; }
    public int AreaPersonalId { get; set; }
    public DateOnly FechaInicio { get; set; } = DateOnly.FromDateTime(DateTime.Today); public DateOnly? FechaObjetivo { get; set; }
    public string? MetricaProgreso { get; set; }
    public string? PorQueEsImportante { get; set; }
    public string? SituacionActual { get; set; }
    public string? ResultadoEsperado { get; set; }
    public string? Obstaculos { get; set; }
    public string? QueFunciona { get; set; }
    public string? MejoraMasPequena { get; set; }
    public string? EvidenciaMejora { get; set; }
    public int DificultadPercibida { get; set; } = 3; public List<GoalSetupActionViewModel> Acciones { get; set; } = [new()];
    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(Titulo)) yield return new("Ingresá un nombre para la meta.", [nameof(Titulo)]);
        if (AreaPersonalId <= 0) yield return new("Seleccioná un área.", [nameof(AreaPersonalId)]);
        if (SubmitIntent != "activate") yield break;
        foreach (var (field, value, message) in new[] { (nameof(Descripcion), Descripcion, "Ingresá una descripción."), (nameof(MetricaProgreso), MetricaProgreso, "Indicá cómo vas a medir el progreso."), (nameof(PorQueEsImportante), PorQueEsImportante, "Explicá por qué esta meta es importante."), (nameof(SituacionActual), SituacionActual, "Describí tu situación actual."), (nameof(ResultadoEsperado), ResultadoEsperado, "Indicá el resultado deseado."), (nameof(Obstaculos), Obstaculos, "Indicá los obstáculos principales."), (nameof(MejoraMasPequena), MejoraMasPequena, "Indicá el primer cambio pequeño."), (nameof(EvidenciaMejora), EvidenciaMejora, "Indicá cómo reconocerás la mejora.") }) if (string.IsNullOrWhiteSpace(value)) yield return new(message, [field]);
        if (DificultadPercibida is < 1 or > 5) yield return new("La dificultad debe estar entre 1 y 5.", [nameof(DificultadPercibida)]);
        var validActions = Acciones.Where(x => !x.IsEmpty).ToList(); if (validActions.Count == 0) yield return new("Agregá al menos una acción válida antes de activar.", [nameof(Acciones)]);
        for (var i = 0; i < Acciones.Count; i++) foreach (var error in Acciones[i].Validate(i)) yield return error;
    }
}
