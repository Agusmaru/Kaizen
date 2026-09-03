using Kaizen.Application.Abstractions;
using Kaizen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kaizen.Infrastructure.Persistence;

public sealed class DailyActionRepository(ApplicationDbContext db) : IDailyActionRepository
{
    public Task<AccionProgramada?> GetAsync(int id, CancellationToken cancellationToken = default) =>
        db.AccionesProgramadas
            .Include(x => x.Registro)
            .Include(x => x.AccionPlanificada)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<List<AccionProgramada>> GetForDateAsync(DateOnly date, CancellationToken cancellationToken = default) =>
        db.AccionesProgramadas
            .Include(x => x.AccionPlanificada)
            .ThenInclude(x => x!.Meta)
            .Where(x => x.FechaProgramada == date)
            .ToListAsync(cancellationToken);

    public void RemoveLog(RegistroAccion log) => db.RegistrosAccion.Remove(log);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
