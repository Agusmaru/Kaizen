using System.ComponentModel.DataAnnotations;
using Kaizen.Domain.Entities;
namespace Kaizen.Web.ViewModels;

public class ReviewActionResultViewModel { public int AccionPlanificadaId { get; set; } public string Nombre { get; set; } = ""; public int Planned { get; set; } public int Completed { get; set; } public int Missed { get; set; } public int Pending { get; set; } public decimal Percentage => Planned == 0 ? 0 : Math.Round(100m * Completed / Planned, 2); public decimal? ExpectedTotal { get; set; } public decimal? ActualTotal { get; set; } public string? Unit { get; set; } }
