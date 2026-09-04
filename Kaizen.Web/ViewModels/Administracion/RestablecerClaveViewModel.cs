using System.ComponentModel.DataAnnotations;

namespace Kaizen.Web.ViewModels.Administracion;

public sealed class RestablecerClaveViewModel
{
    [Required]
    public string UsuarioId { get; set; } = "";

    public string Correo { get; set; } = "";

    [Required(ErrorMessage = "Ingresá una contraseña temporal.")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "La contraseña debe tener al menos 10 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña temporal")]
    public string ClaveTemporal { get; set; } = "";

    [Required(ErrorMessage = "Repetí la contraseña temporal.")]
    [Compare(nameof(ClaveTemporal), ErrorMessage = "Las contraseñas no coinciden.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmacionClave { get; set; } = "";
}
