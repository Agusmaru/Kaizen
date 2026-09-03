using Kaizen.Domain.Entities;

namespace Kaizen.Application.Abstractions;

public interface IDailyActionRepository
{
    Task<AccionProgramada?> GetAsync(int id, CancellationToken cancellationToken = default);
    void RemoveLog(RegistroAccion log);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
