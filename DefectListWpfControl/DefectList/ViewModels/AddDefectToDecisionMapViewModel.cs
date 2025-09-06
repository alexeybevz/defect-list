using System.Windows.Input;
using DefectListWpfControl.DefectList.Commands.DefectToDecisionMapCommands;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class AddDefectToDecisionMapViewModel : ViewModel
    {
        public string FormName => "Создание связки дефект-решение";

        public DefectToDecisionMapDetailsFormViewModel DefectToDecisionMapDetailsFormViewModel { get; }

        public AddDefectToDecisionMapViewModel(DefectToDecisionMapsStore defectToDecisionMapsStore)
        {
            ICommand submitCommand = new AddDefectToDecisionMapCommand(this, defectToDecisionMapsStore);
            DefectToDecisionMapDetailsFormViewModel = new DefectToDecisionMapDetailsFormViewModel(defectToDecisionMapsStore, submitCommand);
        }
    }
}