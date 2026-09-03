using System.ComponentModel.DataAnnotations;

namespace Kaizen.Domain.Entities;

public class AjustePlan
{
    public int Id { get; set; }
    public int RevisionKaizenId { get; set; }
    public RevisionKaizen? RevisionKaizen { get; set; }
    public int? AccionPlanificadaId { get; set; }
    public AccionPlanificada? AccionPlanificada { get; set; }
    public int? NuevaAccionPlanificadaId { get; set; }
    public AccionPlanificada? NuevaAccionPlanificada { get; set; }
    public TipoAjuste Tipo { get; set; }
    [Required]
    public string Descripcion { get; set; } = "";
    public DateOnly FechaVigencia { get; set; }
    public DateOnly? FechaReanudacion { get; set; }
    public string? ValoresAnteriores { get; set; }
    public string? ValoresNuevos { get; set; }
    public string? Motivo { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}