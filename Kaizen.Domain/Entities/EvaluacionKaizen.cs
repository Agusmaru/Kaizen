using System.ComponentModel.DataAnnotations;

namespace Kaizen.Domain.Entities;

public class EvaluacionKaizen
{
    public int Id { get; set; }
    public int MetaId { get; set; }
    public Meta? Meta { get; set; }
    [Required]
    public string DondeEstoy { get; set; } = "";
    [Required]
    public string QueCambiar { get; set; } = "";
    [Required]
    public string Obstaculos { get; set; } = "";
    [Required]
    public string QueFunciona { get; set; } = "";
    [Required]
    public string MejoraMasPequena { get; set; } = "";
    [Required]
    public string EvidenciaMejora { get; set; } = "";
    [Range(1, 5)]
    public int DificultadPercibida { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}