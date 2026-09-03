using Kaizen.Application.Abstractions;
using Kaizen.Domain.Entities;

namespace Kaizen.Application.DailyActions;

public sealed class ReorderDailyActions(IDailyActionRepository repository)
{
    public async Task<DailyActionOrderResult> ExecuteAsync(
        IReadOnlyCollection<int> orderedIds,
        DateOnly expectedDate,
        CancellationToken cancellationToken = default)
    {
        if (orderedIds.Count == 0 || orderedIds.Count != orderedIds.Distinct().Count())
            return new(false, "El orden recibido no es válido.");

        var entries = (await repository.GetForDateAsync(expectedDate, cancellationToken))
            .Where(x => x.AccionPlanificada?.Estado == EstadoAccion.Activa
                && x.AccionPlanificada.Meta?.Estado == EstadoMeta.Activa)
            .ToList();

        if (entries.Count != orderedIds.Count || entries.Select(x => x.Id).ToHashSet().SetEquals(orderedIds) is false)
            return new(false, "No se pudieron validar todas las acciones del día.");

        var entriesById = entries.ToDictionary(x => x.Id);
        var position = 1;
        foreach (var id in orderedIds)
            entriesById[id].Orden = position++;

        await repository.SaveChangesAsync(cancellationToken);
        return new(true, "El orden de tus acciones se guardó.");
    }
}
