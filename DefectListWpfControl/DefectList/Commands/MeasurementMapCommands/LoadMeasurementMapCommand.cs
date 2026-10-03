using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapCommands
{
    public class LoadMeasurementMapCommand : AsyncCommandBase
    {
        private readonly LoadingStateViewModel _loadingStateViewModel;
        private readonly MeasurementMapStore _measurementMapStore;
        private readonly SelectedBomItemStore _selectedBomItemStore;

        public LoadMeasurementMapCommand(LoadingStateViewModel loadingStateViewModel, MeasurementMapStore measurementMapStore, SelectedBomItemStore selectedBomItemStore)
        {
            _loadingStateViewModel = loadingStateViewModel;
            _measurementMapStore = measurementMapStore;
            _selectedBomItemStore = selectedBomItemStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            await _loadingStateViewModel.ExecuteWithLoadingAsync(async () =>
            {
                var bi = _selectedBomItemStore.SelectedBomItem;

                if (bi == null)
                    return;

                await _measurementMapStore.LoadAsync(bi.Id);
            });
        }
    }
}