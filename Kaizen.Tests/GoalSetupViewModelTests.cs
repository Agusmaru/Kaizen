using System.ComponentModel.DataAnnotations;using Kaizen.Web.ViewModels;
namespace Kaizen.Tests;
public class GoalSetupViewModelTests
{
 private static List<ValidationResult> Validate(GoalSetupViewModel vm){var results=new List<ValidationResult>();Validator.TryValidateObject(vm,new(vm),results,true);return results;}
 [Fact]public void Incomplete_goal_can_be_saved_as_draft(){var vm=new GoalSetupViewModel{SubmitIntent="draft",Titulo="Aprender",AreaPersonalId=1,Acciones=[]};Assert.Empty(Validate(vm));}
 [Fact]public void Activation_without_actions_is_rejected(){var vm=Valid();vm.Acciones=[];Assert.Contains(Validate(vm),x=>x.ErrorMessage!.Contains("al menos una acción"));}
 [Fact]public void Valid_goal_can_be_activated(){Assert.Empty(Validate(Valid()));}
 [Fact]public void Invalid_recurrence_is_rejected(){var vm=Valid();vm.Acciones[0].Frecuencia=Kaizen.Domain.Entities.FrecuenciaAccion.DiasSeleccionados;vm.Acciones[0].DiasSemana=null;Assert.Contains(Validate(vm),x=>x.ErrorMessage!.Contains("días"));}
 private static GoalSetupViewModel Valid()=>new(){SubmitIntent="activate",Titulo="Meta",Descripcion="Descripción",AreaPersonalId=1,MetricaProgreso="Sesiones",PorQueEsImportante="Importa",SituacionActual="Actual",ResultadoEsperado="Deseado",Obstaculos="Tiempo",MejoraMasPequena="Cinco minutos",EvidenciaMejora="Registro",Acciones=[new(){Nombre="Practicar",FechaInicio=new(2026,8,4)}]};
}
