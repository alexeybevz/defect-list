using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.DefectToDecisionMapCommands
{
    public class OpenEditDefectToDecisionMapCommand : CommandBase
    {
        private readonly DefectToDecisionMapsViewModel _defectToDecisionMapsViewModel;
        private readonly DefectToDecisionMapsStore _defectToDecisionMapsStore;

        public OpenEditDefectToDecisionMapCommand(DefectToDecisionMapsViewModel defectToDecisionMapsViewModel, DefectToDecisionMapsStore defectToDecisionMapsStore)
        {
            _defectToDecisionMapsViewModel = defectToDecisionMapsViewModel;
            _defectToDecisionMapsStore = defectToDecisionMapsStore;
        }

        public override void Execute(object parameter = null)
        {
            if (_defectToDecisionMapsViewModel.SelectedDefectToDecisionMapViewModel == null)
                return;

            var window = new EditDefectToDecisionMapView()
            {
                DataContext = new EditDefectToDecisionMapViewModel(_defectToDecisionMapsStore, _defectToDecisionMapsViewModel.SelectedDefectToDecisionMapViewModel.MapDefectToDecision)
            };
            _defectToDecisionMapsStore.DefectToDecisionMapUpdated += obj => window.Close();
            window.ShowDialog();
        }
    }
}