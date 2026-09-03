using Kaizen.Domain.Entities;
using Kaizen.Web.Services;using Kaizen.Domain.Rules;
using Kaizen.Web.ViewModels;

namespace Kaizen.Tests;

public class CalendarRangeTests
{
    [Fact]
    public void Visible_range_uses_inclusive_start_and_exclusive_end()
    {
        Assert.True(CalendarRange.TryCreate(new(2026, 8, 1), new(2026, 9, 1), out var range));
        Assert.True(range.Contains(new(2026, 8, 1)));
        Assert.True(range.Contains(new(2026, 8, 31)));
        Assert.False(range.Contains(new(2026, 9, 1)));
    }

    [Fact]
    public void Range_larger_than_safe_limit_is_rejected() =>
        Assert.False(CalendarRange.TryCreate(new(2026, 1, 1), new(2026, 3, 1), out _));

    [Fact]
    public void Recurrence_without_end_date_remains_bounded_by_requested_range()
    {
        var action = new AccionPlanificada
        {
            Estado = EstadoAccion.Activa,
            FechaInicio = new(2026, 8, 1),
            FechaFin = null,
            Frecuencia = FrecuenciaAccion.Diaria
        };
        var start = new DateOnly(2026, 8, 1);
        var occurrences = Enumerable.Range(0, CalendarRange.MaximumDays)
            .Select(start.AddDays)
            .Count(date => KaizenRules.OccursOn(action, date));

        Assert.Equal(CalendarRange.MaximumDays, occurrences);
    }
}
