using System.ComponentModel.DataAnnotations;

namespace Kaizen.Domain.Entities;

public class EventoHistorialMeta
{
    public long Id { get; set; }
    public int MetaId { get; set; }
    public Meta? Meta { get; set; }
    public int? AccionPlanificadaId { get; set; }
    public AccionPlanificada? AccionPlanificada { get; set; }
    public int? RevisionKaizenId { get; set; }
    public RevisionKaizen? RevisionKaizen { get; set; }
    public TipoEventoHistorialMeta Tipo { get; set; }
    [Required]
    public string Descripcion { get; set; } = "";
    public string? ValoresAnteriores { get; set; }
    public string? ValoresNuevos { get; set; }
    public string? Motivo { get; set; }
    public DateTime FechaOcurrencia { get; set; } = DateTime.UtcNow;
}