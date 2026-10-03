using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapDictionaryCommands
{
    public class LoadMeasurementMapDictionaryListCommand : AsyncCommandBase
    {
        private readonly LoadingStateViewModel _loadingState;
        private readonly MeasurementMapDictionaryStore _store;

        public LoadMeasurementMapDictionaryListCommand(
            LoadingStateViewModel loadingState,
            MeasurementMapDictionaryStore store)
        {
            _loadingState = loadingState;
            _store = store;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            await _loadingState.ExecuteWithLoadingAsync(async () =>
            {
                await _store.LoadAllAsync();
            });
        }
    }
}