using Kaizen.Domain.Entities;

namespace Kaizen.Application.DailyActions;

public sealed record DailyActionResult(
    bool Success,
    string Message,
    EstadoCumplimiento Estado = EstadoCumplimiento.Pendiente,
    decimal? ValorReal = null,
    string? Nota = null);
