using System.ComponentModel.DataAnnotations;

namespace Kaizen.Domain.Entities;

public class AreaPersonal
{
    public int Id { get; set; }
    [Required]
    public string Nombre { get; set; } = "";
    public string Color { get; set; } = "#7b9e87";
    public ICollection<Meta> Metas { get; set; } = [];
}