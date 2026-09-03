using Kaizen.Web.Services;using Kaizen.Domain.Rules;using Kaizen.Domain.Entities;
namespace Kaizen.Tests;
public class KaizenRulesTests{
[Theory][InlineData(20,"Reducí")][InlineData(65,"ajuste pequeño")][InlineData(90,"aumentá")]public void Suggestion_matches_completion(decimal value,string expected)=>Assert.Contains(expected,KaizenRules.GetSuggestion(value));
[Fact]public void Consecutive_misses_prioritize_schedule_question()=>Assert.Contains("horario",KaizenRules.GetSuggestion(90,3));
[Fact]public void Selected_weekdays_generate_only_configured_days(){var a=new AccionPlanificada{Estado=EstadoAccion.Activa,FechaInicio=new(2026,8,1),Frecuencia=FrecuenciaAccion.DiasSeleccionados,DiasSemana="1,4"};Assert.True(KaizenRules.OccursOn(a,new(2026,8,3)));Assert.False(KaizenRules.OccursOn(a,new(2026,8,4)));}
[Fact]public void Past_end_date_is_not_scheduled(){var a=new AccionPlanificada{Estado=EstadoAccion.Activa,FechaInicio=new(2026,8,1),FechaFin=new(2026,8,3),Frecuencia=FrecuenciaAccion.Diaria};Assert.False(KaizenRules.OccursOn(a,new(2026,8,4)));}}


