using Kaizen.Application.Abstractions;
using Kaizen.Domain.Entities;

namespace Kaizen.Application.DailyActions;

public sealed class RegisterDailyAction(IDailyActionRepository repository)
{
    public async Task<DailyActionResult> ExecuteAsync(
        int id,
        EstadoCumplimiento status,
        decimal? actualValue,
        string? note,
        DateOnly? expectedDate,
        CancellationToken cancellationToken = default)
    {
        if (status is not (EstadoCumplimiento.Completada or EstadoCumplimiento.NoRealizada))
            return new(false, "Seleccioná un resultado válido.");

        var entry = await repository.GetAsync(id, cancellationToken);
        if (entry is null)
            return new(false, "La acción no existe.");
        if (expectedDate.HasValue && entry.FechaProgramada != expectedDate.Value)
            return new(false, "La acción no corresponde al día de hoy.");
        if (actualValue < 0)
            return new(false, "La cantidad realizada no puede ser negativa.", entry.Estado);

        entry.Estado = status;
        entry.Registro ??= new RegistroAccion { AccionProgramadaId = entry.Id };
        entry.Registro.Estado = status;
        entry.Registro.ValorReal = actualValue;
        entry.Registro.Nota = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        entry.Registro.FechaRegistro = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);

        return new(
            true,
            status == EstadoCumplimiento.Completada
                ? "La acción se marcó como realizada."
                : "La acción se marcó como no realizada.",
            status,
            entry.Registro.ValorReal,
            entry.Registro.Nota);
    }
}
