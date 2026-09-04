namespace Kaizen.Web.ViewModels.Administracion;

public sealed class UsuarioAdministracionViewModel
{
    public string Id { get; set; } = "";
    public string Correo { get; set; } = "";
    public bool EsAdministrador { get; set; }
    public bool DebeCambiarClave { get; set; }
    public bool EstaBloqueado { get; set; }
}
