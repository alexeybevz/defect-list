using System.Windows.Input;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Commands.DefectToDecisionMapCommands;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class EditDefectToDecisionMapViewModel : ViewModel
    {
        public string FormName => $"Изменение связки дефект-решение № {MapDefectToDecision.Id}";

        public MapDefectToDecision MapDefectToDecision { get; }
        public DefectToDecisionMapDetailsFormViewModel DefectToDecisionMapDetailsFormViewModel { get; }

        public EditDefectToDecisionMapViewModel(DefectToDecisionMapsStore defectToDecisionMapsStore, MapDefectToDecision mapDefectToDecision)
        {
            MapDefectToDecision = mapDefectToDecision;

            ICommand submitCommand = new EditDefectToDecisionMapCommand(this, defectToDecisionMapsStore);
            DefectToDecisionMapDetailsFormViewModel = new DefectToDecisionMapDetailsFormViewModel(defectToDecisionMapsStore, submitCommand)
            {
                Defect = mapDefectToDecision.Defect,
                Decision = mapDefectToDecision.Decision,
                IsAllowCombine = mapDefectToDecision.IsAllowCombine,
                StateDetalsId = mapDefectToDecision.StateDetals.Id,
                GroupDefectId = mapDefectToDecision.GroupDefect.Id
            };
        }
    }
}