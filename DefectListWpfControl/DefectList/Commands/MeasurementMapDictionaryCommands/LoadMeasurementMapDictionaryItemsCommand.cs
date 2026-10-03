using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapDictionaryCommands
{
    public class LoadMeasurementMapDictionaryItemsCommand : AsyncCommandBase
    {
        private readonly LoadingStateViewModel _loadingState;
        private readonly MeasurementMapDictionaryStore _store;
        private readonly int _dictionaryId;

        public LoadMeasurementMapDictionaryItemsCommand(
            LoadingStateViewModel loadingState,
            MeasurementMapDictionaryStore store,
            int dictionaryId)
        {
            _loadingState = loadingState;
            _store = store;
            _dictionaryId = dictionaryId;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            await _loadingState.ExecuteWithLoadingAsync(async () =>
            {
                await _store.LoadCurrentAsync(_dictionaryId);
            });
        }
    }
}