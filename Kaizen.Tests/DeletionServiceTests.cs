using Kaizen.Infrastructure.Persistence;
using Kaizen.Domain.Entities;
using Kaizen.Web.Services;using Kaizen.Domain.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kaizen.Tests;

public class DeletionServiceTests
{
 private static ApplicationDbContext Context()=>new(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).ConfigureWarnings(x=>x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);
 private static DeletionService Service(ApplicationDbContext db)=>new(db,NullLogger<DeletionService>.Instance);
 private static async Task<(Meta Meta,AccionPlanificada Action)> Seed(ApplicationDbContext db,EstadoMeta status)
 {
  var area=new AreaPersonal{Nombre="Salud"};var goal=new Meta{Titulo="Meta",Descripcion="Descripción",AreaPersonal=area,PorQueEsImportante="Importa",SituacionActual="Actual",ResultadoEsperado="Esperado",MetricaProgreso="Días",Estado=status,FechaActivacion=status==EstadoMeta.Borrador?null:DateTime.UtcNow};var action=new AccionPlanificada{Meta=goal,Nombre="Caminar",FechaInicio=DateOnly.FromDateTime(DateTime.Today),VigenteDesde=DateOnly.FromDateTime(DateTime.Today),Estado=EstadoAccion.Activa};db.Add(action);await db.SaveChangesAsync();return(goal,action);
 }

 [Fact]public async Task Draft_without_activity_is_deleted_physically(){var db=Context();var(g,_)=await Seed(db,EstadoMeta.Borrador);db.Add(new EventoHistorialMeta{MetaId=g.Id,Tipo=TipoEventoHistorialMeta.Creada,Descripcion="Meta creada."});await db.SaveChangesAsync();var result=await Service(db).DeleteGoalAsync(g.Id,null);Assert.Equal(DeletionOutcome.Deleted,result.Outcome);Assert.Empty(db.Metas);Assert.Empty(db.AccionesPlanificadas);}

 [Fact]public async Task Active_goal_is_archived_and_only_future_unlogged_pending_is_removed(){var db=Context();var(g,a)=await Seed(db,EstadoMeta.Activa);var today=DateOnly.FromDateTime(DateTime.Today);var past=new AccionProgramada{AccionPlanificada=a,FechaProgramada=today.AddDays(-1),Estado=EstadoCumplimiento.Completada,Registro=new(){Estado=EstadoCumplimiento.Completada}};var pending=new AccionProgramada{AccionPlanificada=a,FechaProgramada=today.AddDays(2),Estado=EstadoCumplimiento.Pendiente};var loggedFuture=new AccionProgramada{AccionPlanificada=a,FechaProgramada=today.AddDays(3),Estado=EstadoCumplimiento.Completada,Registro=new(){Estado=EstadoCumplimiento.Completada}};db.AddRange(past,pending,loggedFuture);await db.SaveChangesAsync();var result=await Service(db).DeleteGoalAsync(g.Id,"Finalizada");Assert.Equal(DeletionOutcome.Archived,result.Outcome);Assert.Equal(EstadoMeta.Archivada,g.Estado);Assert.Equal(EstadoAccion.Archivada,a.Estado);Assert.False(await db.AccionesProgramadas.AnyAsync(x=>x.Id==pending.Id));Assert.True(await db.AccionesProgramadas.AnyAsync(x=>x.Id==past.Id));Assert.True(await db.AccionesProgramadas.AnyAsync(x=>x.Id==loggedFuture.Id));Assert.Equal(2,await db.RegistrosAccion.CountAsync());}

 [Fact]public async Task Archiving_is_idempotent(){var db=Context();var(g,_)=await Seed(db,EstadoMeta.Activa);Assert.Equal(DeletionOutcome.Archived,(await Service(db).DeleteGoalAsync(g.Id,null)).Outcome);Assert.Equal(DeletionOutcome.AlreadyArchived,(await Service(db).DeleteGoalAsync(g.Id,null)).Outcome);Assert.Equal(1,await db.EventosHistorialMeta.CountAsync(x=>x.Tipo==TipoEventoHistorialMeta.MetaArchivada));}

 [Fact]public async Task Draft_action_without_activity_is_deleted_physically(){var db=Context();var(g,a)=await Seed(db,EstadoMeta.Borrador);var result=await Service(db).DeleteActionAsync(a.Id,null);Assert.Equal(DeletionOutcome.Deleted,result.Outcome);Assert.Equal(g.Id,result.MetaId);Assert.Empty(db.AccionesPlanificadas);Assert.Single(db.Metas);}

 [Fact]public async Task Schedule_service_does_not_generate_for_archived_goal(){var db=Context();var(g,a)=await Seed(db,EstadoMeta.Activa);await Service(db).DeleteGoalAsync(g.Id,null);var today=DateOnly.FromDateTime(DateTime.Today);await new ScheduleService(db).EnsureScheduledAsync(today,today.AddDays(7));Assert.Empty(db.AccionesProgramadas);Assert.Equal(EstadoAccion.Archivada,a.Estado);}
}
