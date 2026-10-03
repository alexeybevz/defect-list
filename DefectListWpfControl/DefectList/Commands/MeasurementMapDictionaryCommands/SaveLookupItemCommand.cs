using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapDictionaryCommands
{
    public class SaveLookupItemCommand : AsyncCommandBase
    {
        private readonly LoadingStateViewModel _loadingState;
        private readonly LookupStore _store;
        private readonly LookupDictionaryViewModel _vm;

        public SaveLookupItemCommand(
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
            return _vm.SelectedItem != null && _vm.SelectedItem.HasChanges;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            await _loadingState.ExecuteWithLoadingAsync(async () =>
            {
                var item = _vm.SelectedItem;
                await _store.SaveAsync(item.Id, item.Name, item.IsUsed, _vm.User?.Name);
            });
        }
    }
}