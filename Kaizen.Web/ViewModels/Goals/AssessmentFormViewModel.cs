using System.ComponentModel.DataAnnotations;

namespace Kaizen.Web.ViewModels;

public class AssessmentFormViewModel
{
    public int MetaId { get; set; }
    public string GoalTitle { get; set; } = "";

    [Required(ErrorMessage = "Indicá dónde estás actualmente.")]
    public string DondeEstoy { get; set; } = "";

    [Required(ErrorMessage = "Indicá qué querés cambiar.")]
    public string QueCambiar { get; set; } = "";

    [Required(ErrorMessage = "Indicá qué obstáculos encontrás.")]
    public string Obstaculos { get; set; } = "";

    [Required(ErrorMessage = "Indicá qué estás haciendo bien.")]
    public string QueFunciona { get; set; } = "";

    [Required(ErrorMessage = "Indicá la mejora más pequeña que podés comenzar hoy.")]
    public string MejoraMasPequena { get; set; } = "";

    [Required(ErrorMessage = "Indicá cómo vas a saber si estás mejorando.")]
    public string EvidenciaMejora { get; set; } = "";

    [Range(1, 5, ErrorMessage = "La dificultad debe estar entre 1 y 5.")]
    public int DificultadPercibida { get; set; } = 3;
}
