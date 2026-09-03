using System.ComponentModel.DataAnnotations;
using Kaizen.Domain.Entities;
using Kaizen.Web.ViewModels;
namespace Kaizen.Tests;
public class ActionFormViewModelTests
{
 [Fact] public void Description_is_optional(){var vm=Valid();vm.Descripcion=null;Assert.Empty(Validate(vm));}
 [Fact] public void Name_is_required_in_spanish(){var vm=Valid();vm.Nombre="";Assert.Contains(Validate(vm),x=>x.ErrorMessage=="Ingresá el nombre de la acción.");}
 [Fact] public void Selected_weekdays_requires_a_day(){var vm=Valid();vm.Frecuencia=FrecuenciaAccion.DiasSeleccionados;Assert.Contains(Validate(vm),x=>x.ErrorMessage=="Seleccioná al menos un día de la semana.");}
 private static ActionFormViewModel Valid()=>new(){MetaId=2,Nombre="Practicar",FechaInicio=new(2026,8,3),DificultadEstimada=1};
 private static List<ValidationResult> Validate(ActionFormViewModel vm){var results=new List<ValidationResult>();Validator.TryValidateObject(vm,new(vm),results,true);return results;}
}
