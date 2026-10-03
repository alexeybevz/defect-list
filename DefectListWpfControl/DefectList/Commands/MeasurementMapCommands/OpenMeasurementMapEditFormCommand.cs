using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapCommands
{
    public class OpenMeasurementMapEditFormCommand : AsyncCommandBase
    {
        private readonly MeasurementMapViewModel _measurementMapViewModel;
        private readonly DefectListItemViewModel _defectListItemViewModel;

        public OpenMeasurementMapEditFormCommand(MeasurementMapViewModel measurementMapViewModel,
            DefectListItemViewModel defectListItemViewModel)
        {
            _measurementMapViewModel = measurementMapViewModel;
            _defectListItemViewModel = defectListItemViewModel;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            _measurementMapViewModel.LoadMapCommand?.Execute();
            var window = new MeasurementMapWindow() { DataContext = _measurementMapViewModel };
            window.Closed += (sender, args) => _defectListItemViewModel.LoadBomItemCommand?.Execute();
            window.ShowDialog();
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) &&
                   PermissionsStore.IsSuperUser &&
                   _measurementMapViewModel.IsEditable;
        }
    }
}