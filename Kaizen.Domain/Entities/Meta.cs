using System.ComponentModel.DataAnnotations;

namespace Kaizen.Domain.Entities;

public class Meta
{
    public int Id { get; set; }
    [Required]
    public string UsuarioId { get; set; } = "";
    [Required]
    public string Titulo { get; set; } = "";
    [Required]
    public string Descripcion { get; set; } = "";
    public int AreaPersonalId { get; set; }
    public AreaPersonal? AreaPersonal { get; set; }
    [Required]
    public string PorQueEsImportante { get; set; } = "";
    [Required]
    public string SituacionActual { get; set; } = "";
    [Required]
    public string ResultadoEsperado { get; set; } = "";
    public DateOnly FechaInicio { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly? FechaObjetivo { get; set; }
    [Required]
    public string MetricaProgreso { get; set; } = "";
    public EstadoMeta Estado { get; set; } = EstadoMeta.Borrador;
    public FrecuenciaRevision FrecuenciaRevision { get; set; } = FrecuenciaRevision.Semanal;
    public DateOnly? FechaProximaRevision { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaActivacion { get; set; }
    public DateTime? FechaArchivo { get; set; }
    public string? MotivoArchivo { get; set; }
    public EvaluacionKaizen? Evaluacion { get; set; }
    public ICollection<AccionPlanificada> Acciones { get; set; } = [];
    public ICollection<RevisionKaizen> Revisiones { get; set; } = [];
    public ICollection<EventoHistorialMeta> EventosHistorial { get; set; } = [];
}
