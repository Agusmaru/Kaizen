using System.ComponentModel.DataAnnotations;

namespace Kaizen.Web.ViewModels;

public sealed record ActionVersionViewModel(int Id, int Version, DateOnly From, DateOnly? To, string Configuration, string? Motivo, string Origin, string Estado);
