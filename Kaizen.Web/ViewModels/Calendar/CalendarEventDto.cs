namespace Kaizen.Web.ViewModels;

public sealed record CalendarEventDto(
    int Id,
    string Titulo,
    string GoalTitle,
    DateOnly Date,
    TimeOnly? Hora,
    string Estado,
    int MetaId,
    string DetailUrl,
    string? Nota);
