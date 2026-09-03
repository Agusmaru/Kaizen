using Kaizen.Application.Usuarios;
using Kaizen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kaizen.Infrastructure.Persistence;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IUsuarioActual? usuarioActual = null) : DbContext(options)
{
    private bool FiltrarPorUsuario => usuarioActual?.EstaAutenticado == true;
    private string? UsuarioIdActual => usuarioActual?.UsuarioId;

    public DbSet<AreaPersonal> AreasPersonales => Set<AreaPersonal>();
    public DbSet<Meta> Metas => Set<Meta>();
    public DbSet<EvaluacionKaizen> EvaluacionesKaizen => Set<EvaluacionKaizen>();
    public DbSet<AccionPlanificada> AccionesPlanificadas => Set<AccionPlanificada>();
    public DbSet<AccionProgramada> AccionesProgramadas => Set<AccionProgramada>();
    public DbSet<RegistroAccion> RegistrosAccion => Set<RegistroAccion>();
    public DbSet<RevisionKaizen> RevisionesKaizen => Set<RevisionKaizen>();
    public DbSet<AjustePlan> AjustesPlan => Set<AjustePlan>();
    public DbSet<EventoHistorialMeta> EventosHistorialMeta => Set<EventoHistorialMeta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AreaPersonal>().ToTable(nameof(AreaPersonal));
        modelBuilder.Entity<Meta>().ToTable(nameof(Meta));
        modelBuilder.Entity<EvaluacionKaizen>().ToTable(nameof(EvaluacionKaizen));
        modelBuilder.Entity<AccionPlanificada>().ToTable(nameof(AccionPlanificada));
        modelBuilder.Entity<AccionProgramada>().ToTable(nameof(AccionProgramada));
        modelBuilder.Entity<RegistroAccion>().ToTable(nameof(RegistroAccion));
        modelBuilder.Entity<RevisionKaizen>().ToTable(nameof(RevisionKaizen));
        modelBuilder.Entity<AjustePlan>().ToTable(nameof(AjustePlan));
        modelBuilder.Entity<EventoHistorialMeta>().ToTable(nameof(EventoHistorialMeta));

        modelBuilder.Entity<Meta>().HasIndex(x => x.UsuarioId);
        modelBuilder.Entity<Meta>().HasQueryFilter(x => !FiltrarPorUsuario || x.UsuarioId == UsuarioIdActual);
        modelBuilder.Entity<EvaluacionKaizen>().HasQueryFilter(x => !FiltrarPorUsuario || x.Meta!.UsuarioId == UsuarioIdActual);
        modelBuilder.Entity<AccionPlanificada>().HasQueryFilter(x => !FiltrarPorUsuario || x.Meta!.UsuarioId == UsuarioIdActual);
        modelBuilder.Entity<AccionProgramada>().HasQueryFilter(x => !FiltrarPorUsuario || x.AccionPlanificada!.Meta!.UsuarioId == UsuarioIdActual);
        modelBuilder.Entity<RegistroAccion>().HasQueryFilter(x => !FiltrarPorUsuario || x.AccionProgramada!.AccionPlanificada!.Meta!.UsuarioId == UsuarioIdActual);
        modelBuilder.Entity<RevisionKaizen>().HasQueryFilter(x => !FiltrarPorUsuario || x.Meta!.UsuarioId == UsuarioIdActual);
        modelBuilder.Entity<AjustePlan>().HasQueryFilter(x => !FiltrarPorUsuario || x.RevisionKaizen!.Meta!.UsuarioId == UsuarioIdActual);
        modelBuilder.Entity<EventoHistorialMeta>().HasQueryFilter(x => !FiltrarPorUsuario || x.Meta!.UsuarioId == UsuarioIdActual);

        modelBuilder.Entity<Meta>()
            .HasOne(x => x.Evaluacion)
            .WithOne(x => x.Meta)
            .HasForeignKey<EvaluacionKaizen>(x => x.MetaId);

        modelBuilder.Entity<AccionProgramada>()
            .HasIndex(x => new { x.AccionPlanificadaId, x.FechaProgramada })
            .IsUnique();
        modelBuilder.Entity<AccionProgramada>().HasIndex(x => x.FechaProgramada);
        modelBuilder.Entity<AccionProgramada>().Property(x => x.Orden).HasDefaultValue(0);
        modelBuilder.Entity<AccionProgramada>()
            .HasOne(x => x.Registro)
            .WithOne(x => x.AccionProgramada)
            .HasForeignKey<RegistroAccion>(x => x.AccionProgramadaId);

        modelBuilder.Entity<AccionPlanificada>()
            .HasOne(x => x.VersionAnterior)
            .WithMany()
            .HasForeignKey(x => x.VersionAnteriorId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<AccionPlanificada>()
            .HasOne(x => x.RevisionOrigen)
            .WithMany()
            .HasForeignKey(x => x.RevisionOrigenId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<AccionPlanificada>()
            .HasIndex(x => new { x.SerieAccionId, x.VigenteDesde });

        modelBuilder.Entity<AjustePlan>()
            .HasOne(x => x.AccionPlanificada)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<AjustePlan>()
            .HasOne(x => x.NuevaAccionPlanificada)
            .WithMany()
            .HasForeignKey(x => x.NuevaAccionPlanificadaId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<RevisionKaizen>()
            .HasIndex(x => new { x.MetaId, x.InicioPeriodo, x.FinPeriodo })
            .IsUnique();

        modelBuilder.Entity<EventoHistorialMeta>()
            .HasIndex(x => new { x.MetaId, x.FechaOcurrencia });
        modelBuilder.Entity<EventoHistorialMeta>()
            .HasOne(x => x.AccionPlanificada)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<EventoHistorialMeta>()
            .HasOne(x => x.RevisionKaizen)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<RevisionKaizen>().Property(x => x.PorcentajeCumplimiento).HasPrecision(5, 2);
        modelBuilder.Entity<AccionPlanificada>().Property(x => x.CantidadObjetivo).HasPrecision(18, 2);
        modelBuilder.Entity<RegistroAccion>().Property(x => x.ValorReal).HasPrecision(18, 2);
    }
}
