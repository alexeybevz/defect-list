using System.Threading.Tasks;
using System.Windows;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapDictionaryCommands
{
    public class CreateMeasurementMapDictionaryVersionCommand : AsyncCommandBase
    {
        private readonly LoadingStateViewModel _loadingState;
        private readonly MeasurementMapDictionaryStore _store;
        private readonly MeasurementMapDictionaryListViewModel _vm;

        public CreateMeasurementMapDictionaryVersionCommand(
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
            return _vm.SelectedDictionary != null;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            var selected = _vm.SelectedDictionary;
            if (selected == null) return;

            var confirm = MessageBox.Show(
                $"Создать новую версию на основе справочника Id={selected.Id}, Code_LSF82={selected.Code_LSF82}, версия {selected.Version}?\n" +
                "Текущий активный справочник будет архивирован, все строки будут скопированы в новую версию.",
                "Подтверждение создания новой версии",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes)
                return;

            await _loadingState.ExecuteWithLoadingAsync(async () =>
            {
                var newId = await _store.CreateVersionAsync(selected.Id, _vm.User?.Name);
                // Открытие формы строк новой версии запрашивается через событие
                _vm.RequestOpenItems(newId);
            });
        }
    }
}