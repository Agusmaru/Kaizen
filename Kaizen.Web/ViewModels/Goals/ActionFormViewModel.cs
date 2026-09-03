using System.ComponentModel.DataAnnotations;
using Kaizen.Domain.Entities;
namespace Kaizen.Web.ViewModels;

public class ActionFormViewModel : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "La meta asociada no es válida.")] public int MetaId { get; set; }
    [Required(ErrorMessage = "Ingresá el nombre de la acción.")][StringLength(160, ErrorMessage = "El nombre no puede superar los 160 caracteres.")] public string Nombre { get; set; } = "";
    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")] public string? Descripcion { get; set; }
    public FrecuenciaAccion Frecuencia { get; set; }
    public string? DiasSemana { get; set; }
    public TimeOnly? Hora { get; set; }
    [Required(ErrorMessage = "Ingresá la fecha de inicio.")] public DateOnly? FechaInicio { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly? FechaFin { get; set; }
    [Range(1, 5, ErrorMessage = "La dificultad debe estar entre 1 y 5.")] public int DificultadEstimada { get; set; } = 1;
    [Range(typeof(decimal), "0", "9999999999999999", ErrorMessage = "La cantidad no puede ser negativa.")] public decimal? CantidadObjetivo { get; set; }
    [StringLength(50, ErrorMessage = "La unidad no puede superar los 50 caracteres.")] public string? UnidadMetrica { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext context) { if (Frecuencia == FrecuenciaAccion.DiasSeleccionados && string.IsNullOrWhiteSpace(DiasSemana)) yield return new("Seleccioná al menos un día de la semana.", [nameof(DiasSemana)]); if (FechaInicio.HasValue && FechaFin.HasValue && FechaFin < FechaInicio) yield return new("La fecha de finalización no puede ser anterior a la fecha de inicio.", [nameof(FechaFin)]); }
}
