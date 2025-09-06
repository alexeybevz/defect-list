using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.DefectToDecisionMapCommands
{
    public class OpenAddDefectToDecisionMapCommand : CommandBase
    {
        private readonly DefectToDecisionMapsStore _defectToDecisionMapsStore;

        public OpenAddDefectToDecisionMapCommand(DefectToDecisionMapsStore defectToDecisionMapsStore)
        {
            _defectToDecisionMapsStore = defectToDecisionMapsStore;
        }

        public override void Execute(object parameter = null)
        {
            var window = new AddDefectToDecisionMapView()
            {
                DataContext = new AddDefectToDecisionMapViewModel(_defectToDecisionMapsStore)
            };
            _defectToDecisionMapsStore.DefectToDecisionMapAdded += obj => window.Close();
            window.ShowDialog();
        }
    }
}