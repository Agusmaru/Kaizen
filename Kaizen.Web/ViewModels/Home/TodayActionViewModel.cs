using Kaizen.Domain.Entities;

namespace Kaizen.Web.ViewModels;

public sealed class TodayActionViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = ""; public int MetaId { get; set; }
    public string GoalTitle { get; set; } = ""; public EstadoCumplimiento Estado { get; set; }
    public decimal? CantidadObjetivo { get; set; }
    public string? UnidadMetrica { get; set; }
    public TimeOnly? Hora { get; set; }
    public decimal? ValorReal { get; set; }
    public string? Nota { get; set; }
    public string StatusLabel => Estado switch { EstadoCumplimiento.Completada => "Realizada", EstadoCumplimiento.NoRealizada => "No realizada", _ => "Pendiente" };
    public string StatusColor => Estado switch { EstadoCumplimiento.Completada => "success", EstadoCumplimiento.NoRealizada => "danger", _ => "secondary" };
}
