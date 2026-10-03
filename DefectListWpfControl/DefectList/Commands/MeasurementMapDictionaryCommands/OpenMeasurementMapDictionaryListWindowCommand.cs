using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Factories;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapDictionaryCommands
{
    public class OpenMeasurementMapDictionaryListWindowCommand : AsyncCommandBase
    {
        private readonly MeasurementMapDictionaryListWindowFactory _measurementMapDictionaryListWindowFactory;

        public OpenMeasurementMapDictionaryListWindowCommand(
            MeasurementMapDictionaryListWindowFactory measurementMapDictionaryListWindowFactory)
        {
            _measurementMapDictionaryListWindowFactory = measurementMapDictionaryListWindowFactory;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            await _measurementMapDictionaryListWindowFactory.OpenAsync();
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) && PermissionsStore.IsSuperUser;
        }
    }
}