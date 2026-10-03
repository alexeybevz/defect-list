using System.Threading.Tasks;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapDictionaryCommands
{
    public class OpenEditDictionaryItemCommand : AsyncCommandBase
    {
        private readonly MeasurementMapDictionaryItemsViewModel _vm;

        public OpenEditDictionaryItemCommand(MeasurementMapDictionaryItemsViewModel vm)
        {
            _vm = vm;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            if (_vm.SelectedItem == null) return;
            _vm.RequestOpenItemEdit(_vm.SelectedItem.Model);
        }

        public override bool CanExecute(object parameter = null)
        {
            return _vm.SelectedItem != null &&
                   _vm.IsDictionaryActive;
        }
    }
}