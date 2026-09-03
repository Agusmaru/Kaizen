using System.ComponentModel.DataAnnotations;

namespace Kaizen.Web.ViewModels;

public sealed class CambiarClaveViewModel
{
    [Required(ErrorMessage = "Ingresá la contraseña actual.")]
    [DataType(DataType.Password)]
    public string ClaveActual { get; set; } = "";

    [Required(ErrorMessage = "Ingresá la nueva contraseña.")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "La contraseña debe tener al menos 10 caracteres.")]
    [DataType(DataType.Password)]
    public string NuevaClave { get; set; } = "";

    [Required(ErrorMessage = "Repetí la nueva contraseña.")]
    [Compare(nameof(NuevaClave), ErrorMessage = "Las contraseñas no coinciden.")]
    [DataType(DataType.Password)]
    public string ConfirmacionNuevaClave { get; set; } = "";
}
