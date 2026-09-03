using System.ComponentModel.DataAnnotations;

namespace Kaizen.Web.ViewModels;

public sealed class ActionEditViewModel : ActionFormViewModel
{
    public int ActionId { get; set; }
    public int OriginalVersion { get; set; }
    public DateOnly FechaVigencia { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    [Required(ErrorMessage = "Explicá el motivo de la modificación.")][StringLength(500, ErrorMessage = "El motivo no puede superar los 500 caracteres.")] public string Motivo { get; set; } = "";
    public string SubmitIntent { get; set; } = "preview";
    public bool CanEdit { get; set; } = true;
    public bool DirectUpdate { get; set; }
    public bool HasRegisteredFuture { get; set; }
    public List<string> Changes { get; set; } = [];
    public List<ActionVersionViewModel> Versions { get; set; } = [];
}
