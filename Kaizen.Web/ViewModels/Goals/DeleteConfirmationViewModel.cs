namespace Kaizen.Web.ViewModels;

public sealed class DeleteConfirmationViewModel
{
    public int Id { get; init; }
    public int? MetaId { get; init; }
    public required string Titulo { get; init; }
    public bool WillArchive { get; init; }
    public int Acciones { get; init; }
    public int Occurrences { get; init; }
    public int Logs { get; init; }
    public int Revisiones { get; init; }
    public string? Motivo { get; set; }
}
