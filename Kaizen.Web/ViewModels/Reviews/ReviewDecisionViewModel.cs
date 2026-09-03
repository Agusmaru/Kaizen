using System.ComponentModel.DataAnnotations;
using Kaizen.Domain.Entities;
namespace Kaizen.Web.ViewModels;

public class ReviewDecisionViewModel { public int AccionPlanificadaId { get; set; } public string ActionName { get; set; } = ""; public int CurrentVersion { get; set; } public string CurrentConfiguration { get; set; } = ""; public TipoAjuste Tipo { get; set; } public DateOnly FechaVigencia { get; set; } = DateOnly.FromDateTime(DateTime.Today).AddDays(1); public string? Motivo { get; set; } public string? NewName { get; set; } public FrecuenciaAccion NewFrequency { get; set; } public string? NewWeekdays { get; set; } public TimeOnly? NewTime { get; set; } public decimal? NewTargetAmount { get; set; } public string? NewMetricUnit { get; set; } public DateOnly? NewEndDate { get; set; } public DateOnly? FechaReanudacion { get; set; } }
