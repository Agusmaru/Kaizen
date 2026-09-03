using Kaizen.Domain.Entities;
namespace Kaizen.Domain.Rules;

public static class KaizenRules
{
    public static string GetSuggestion(decimal completion, int misses = 0) => misses >= 3 ? "Revisá si el horario o la frecuencia siguen siendo adecuados." : completion < 50 ? "Reducí la dificultad, la duración o la frecuencia." : completion < 80 ? "Mantené el plan o realizá un ajuste pequeño." : "Si se mantiene, aumentá gradualmente el desafío.";
    public static bool OccursOn(AccionPlanificada item, DateOnly date) { var effectiveFrom = item.VigenteDesde == default ? item.FechaInicio : item.VigenteDesde; if (item.Estado != EstadoAccion.Activa || date < item.FechaInicio || date < effectiveFrom || date > (item.FechaFin ?? DateOnly.MaxValue) || date > (item.VigenteHasta ?? DateOnly.MaxValue)) return false; return item.Frecuencia switch { FrecuenciaAccion.Diaria => true, FrecuenciaAccion.DiasSeleccionados => (item.DiasSemana ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).Contains(((int)date.DayOfWeek).ToString()), FrecuenciaAccion.Semanal => date.DayOfWeek == item.FechaInicio.DayOfWeek, FrecuenciaAccion.FechaEspecifica => date == item.FechaInicio, _ => false }; }
}
