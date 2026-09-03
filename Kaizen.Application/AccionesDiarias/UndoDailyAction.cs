using Kaizen.Application.Abstractions;
using Kaizen.Domain.Entities;

namespace Kaizen.Application.DailyActions;

public sealed class UndoDailyAction(IDailyActionRepository repository)
{
    public async Task<DailyActionResult> ExecuteAsync(
        int id,
        DateOnly expectedDate,
        CancellationToken cancellationToken = default)
    {
        var entry = await repository.GetAsync(id, cancellationToken);
        if (entry is null || entry.FechaProgramada != expectedDate)
            return new(false, "La acción no corresponde al día de hoy.");

        if (entry.Registro is not null)
            repository.RemoveLog(entry.Registro);
        entry.Registro = null;
        entry.Estado = EstadoCumplimiento.Pendiente;
        await repository.SaveChangesAsync(cancellationToken);
        return new(true, "El registro se deshizo correctamente.");
    }
}
