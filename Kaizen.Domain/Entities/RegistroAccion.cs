using System.ComponentModel.DataAnnotations;

namespace Kaizen.Domain.Entities;

public class RegistroAccion
{
    public int Id { get; set; }
    public int AccionProgramadaId { get; set; }
    public AccionProgramada? AccionProgramada { get; set; }
    public EstadoCumplimiento Estado { get; set; }
    public string? Nota { get; set; }
    public decimal? ValorReal { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}