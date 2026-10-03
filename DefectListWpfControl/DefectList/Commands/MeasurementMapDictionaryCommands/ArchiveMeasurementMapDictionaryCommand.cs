using System.Threading.Tasks;
using System.Windows;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapDictionaryCommands
{
    public class ArchiveMeasurementMapDictionaryCommand : AsyncCommandBase
    {
        private readonly LoadingStateViewModel _loadingState;
        private readonly MeasurementMapDictionaryStore _store;
        private readonly MeasurementMapDictionaryListViewModel _vm;

        public ArchiveMeasurementMapDictionaryCommand(
            LoadingStateViewModel loadingState,
            MeasurementMapDictionaryStore store,
            MeasurementMapDictionaryListViewModel vm)
        {
            _loadingState = loadingState;
            _store = store;
            _vm = vm;
        }

        public override bool CanExecute(object parameter = null)
        {
            return _vm.SelectedDictionary != null && _vm.SelectedDictionary.IsActive;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            var selected = _vm.SelectedDictionary;
            if (selected == null) return;

            var confirm = MessageBox.Show(
                $"Архивировать справочник Id={selected.Id}, Code_LSF82={selected.Code_LSF82}, версия {selected.Version}?\n" +
                "Действующие экземпляры карт измерений останутся без изменений.",
                "Подтверждение архивирования",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            await _loadingState.ExecuteWithLoadingAsync(async () =>
            {
                await _store.ArchiveAsync(selected.Id, _vm.User?.Name);
            });
        }
    }
}