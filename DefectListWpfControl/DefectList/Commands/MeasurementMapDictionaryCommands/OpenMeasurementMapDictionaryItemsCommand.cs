using System.Threading.Tasks;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapDictionaryCommands
{
    public class OpenMeasurementMapDictionaryItemsCommand : AsyncCommandBase
    {
        private readonly MeasurementMapDictionaryListViewModel _vm;

        public OpenMeasurementMapDictionaryItemsCommand(MeasurementMapDictionaryListViewModel vm)
        {
            _vm = vm;
        }

        public override bool CanExecute(object parameter = null)
        {
            return _vm.SelectedDictionary != null;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            if (_vm.SelectedDictionary == null) return;
            _vm.RequestOpenItems(_vm.SelectedDictionary.Id);
        }
    }
}