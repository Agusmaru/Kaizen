using System.ComponentModel.DataAnnotations;

namespace Kaizen.Web.ViewModels;

public sealed class RegistroViewModel
{
    [Required(ErrorMessage = "Ingresá tu correo.")]
    [EmailAddress(ErrorMessage = "Ingresá un correo válido.")]
    public string Correo { get; set; } = "";

    [Required(ErrorMessage = "Ingresá una contraseña.")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "La contraseña debe tener al menos 10 caracteres.")]
    [DataType(DataType.Password)]
    public string Clave { get; set; } = "";

    [Required(ErrorMessage = "Repetí la contraseña.")]
    [Compare(nameof(Clave), ErrorMessage = "Las contraseñas no coinciden.")]
    [DataType(DataType.Password)]
    public string ConfirmacionClave { get; set; } = "";
}
