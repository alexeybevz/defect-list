using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapDictionaryCommands
{
    public class LoadLookupItemsCommand : AsyncCommandBase
    {
        private readonly LoadingStateViewModel _loadingState;
        private readonly LookupStore _store;
        private readonly LookupDictionaryViewModel _vm;

        public LoadLookupItemsCommand(
            LoadingStateViewModel loadingState,
            LookupStore store,
            LookupDictionaryViewModel vm)
        {
            _loadingState = loadingState;
            _store = store;
            _vm = vm;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            await _loadingState.ExecuteWithLoadingAsync(async () =>
            {
                await _store.LoadAsync();
            });
        }
    }
}