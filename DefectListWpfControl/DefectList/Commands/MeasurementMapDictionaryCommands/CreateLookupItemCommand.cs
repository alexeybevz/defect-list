using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapDictionaryCommands
{
    public class CreateLookupItemCommand : AsyncCommandBase
    {
        private readonly LoadingStateViewModel _loadingState;
        private readonly LookupStore _store;
        private readonly LookupDictionaryViewModel _vm;

        public CreateLookupItemCommand(
            LoadingStateViewModel loadingState,
            LookupStore store,
            LookupDictionaryViewModel vm)
        {
            _loadingState = loadingState;
            _store = store;
            _vm = vm;
        }

        public override bool CanExecute(object parameter = null)
        {
            return !string.IsNullOrWhiteSpace(_vm.NewItemName);
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            await _loadingState.ExecuteWithLoadingAsync(async () =>
            {
                await _store.CreateAsync(_vm.NewItemName, _vm.User?.Name);
                _vm.NewItemName = null;
            });
        }
    }
}