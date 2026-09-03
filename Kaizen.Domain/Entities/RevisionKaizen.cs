using System.ComponentModel.DataAnnotations;

namespace Kaizen.Domain.Entities;

public class RevisionKaizen
{
    public int Id { get; set; }
    public int MetaId { get; set; }
    public Meta? Meta { get; set; }
    public DateOnly InicioPeriodo { get; set; }
    public DateOnly FinPeriodo { get; set; }
    public decimal PorcentajeCumplimiento { get; set; }
    [Required]
    public string QueFunciono { get; set; } = "";
    [Required]
    public string QueDificulto { get; set; } = "";
    public bool FueDemasiadoDificil { get; set; }
    [Required]
    public string AjustePequeno { get; set; } = "";
    public string? SugerenciaAutomatica { get; set; }
    [Required]
    public string Aprendizaje { get; set; } = "";
    [Required]
    public string CambioProximoPeriodo { get; set; } = "";
    public string? NotasAdicionales { get; set; }
    public EstadoMeta EstadoMetaInicial { get; set; }
    public EstadoMeta EstadoMetaFinal { get; set; }
    public DateOnly FechaProximaRevision { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public ICollection<AjustePlan> Ajustes { get; set; } = [];
}