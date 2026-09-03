using System.ComponentModel.DataAnnotations;

namespace Kaizen.Domain.Entities;

public class AccionPlanificada
{
    public int Id { get; set; }
    public int MetaId { get; set; }
    public Meta? Meta { get; set; }
    [Required]
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public FrecuenciaAccion Frecuencia { get; set; }
    public string? DiasSemana { get; set; }
    public TimeOnly? Hora { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    [Range(1, 5)]
    public int DificultadEstimada { get; set; } = 1;
    public decimal? CantidadObjetivo { get; set; }
    public string? UnidadMetrica { get; set; }
    public EstadoAccion Estado { get; set; } = EstadoAccion.Activa;
    public DateTime? FechaArchivo { get; set; }
    public string? MotivoArchivo { get; set; }
    public Guid SerieAccionId { get; set; } = Guid.NewGuid();
    public int Version { get; set; } = 1;
    public DateOnly VigenteDesde { get; set; }
    public DateOnly? VigenteHasta { get; set; }
    public int? VersionAnteriorId { get; set; }
    public AccionPlanificada? VersionAnterior { get; set; }
    public string? MotivoCambio { get; set; }
    public int? RevisionOrigenId { get; set; }
    public RevisionKaizen? RevisionOrigen { get; set; }
    public ICollection<AccionProgramada> AccionesProgramadas { get; set; } = [];
}