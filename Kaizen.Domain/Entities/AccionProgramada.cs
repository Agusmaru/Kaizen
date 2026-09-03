using System.ComponentModel.DataAnnotations;

namespace Kaizen.Domain.Entities;

public class AccionProgramada
{
    public int Id { get; set; }
    public int AccionPlanificadaId { get; set; }
    public AccionPlanificada? AccionPlanificada { get; set; }
    public DateOnly FechaProgramada { get; set; }
    public int Orden { get; set; }
    public EstadoCumplimiento Estado { get; set; }
    public RegistroAccion? Registro { get; set; }
}
