namespace Kaizen.Infrastructure.Identidad;

public sealed class SesionUsuario
{
    public Guid Id { get; set; }
    public string UsuarioId { get; set; } = "";
    public UsuarioAplicacion? Usuario { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime UltimaActividad { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public DateTime? FechaRevocacion { get; set; }
    public string? DireccionIp { get; set; }
    public string? Dispositivo { get; set; }
}
