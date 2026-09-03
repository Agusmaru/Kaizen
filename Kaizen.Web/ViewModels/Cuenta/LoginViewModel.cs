using System.ComponentModel.DataAnnotations;

namespace Kaizen.Web.ViewModels;

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "Ingresá tu correo.")]
    [EmailAddress(ErrorMessage = "Ingresá un correo válido.")]
    public string Correo { get; set; } = "";

    [Required(ErrorMessage = "Ingresá tu contraseña.")]
    [DataType(DataType.Password)]
    public string Clave { get; set; } = "";

    public bool Recordarme { get; set; }
    public string? Retorno { get; set; }
}
