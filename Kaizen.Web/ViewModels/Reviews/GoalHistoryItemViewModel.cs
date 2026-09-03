using System.ComponentModel.DataAnnotations;
using Kaizen.Domain.Entities;
namespace Kaizen.Web.ViewModels;

public record GoalHistoryItemViewModel(DateTime FechaOcurrencia, string Tipo, string Descripcion, string? ActionName, string? ValoresAnteriores, string? ValoresNuevos, string? Motivo);
