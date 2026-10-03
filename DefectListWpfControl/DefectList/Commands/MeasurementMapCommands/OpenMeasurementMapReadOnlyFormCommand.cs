using System.Threading.Tasks;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapCommands
{
    public class OpenMeasurementMapReadOnlyFormCommand : AsyncCommandBase
    {
        private readonly MeasurementMapReadOnlyViewModel _measurementMapReadOnlyViewModel;

        public OpenMeasurementMapReadOnlyFormCommand(MeasurementMapReadOnlyViewModel measurementMapReadOnlyViewModel)
        {
            _measurementMapReadOnlyViewModel = measurementMapReadOnlyViewModel;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            _measurementMapReadOnlyViewModel.LoadMapCommand?.Execute();
            var window = new MeasurementMapReadOnlyWindow() { DataContext = _measurementMapReadOnlyViewModel };
            window.ShowDialog();
        }
    }
}