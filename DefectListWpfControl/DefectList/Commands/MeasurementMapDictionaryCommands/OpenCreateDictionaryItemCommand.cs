using System.Threading.Tasks;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapDictionaryCommands
{
    public class OpenCreateDictionaryItemCommand : AsyncCommandBase
    {
        private readonly MeasurementMapDictionaryItemsViewModel _vm;

        public OpenCreateDictionaryItemCommand(MeasurementMapDictionaryItemsViewModel vm)
        {
            _vm = vm;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            _vm.RequestOpenItemEdit(null);
        }

        public override bool CanExecute(object parameter = null)
        {
            return _vm.IsDictionaryActive;
        }
    }
}