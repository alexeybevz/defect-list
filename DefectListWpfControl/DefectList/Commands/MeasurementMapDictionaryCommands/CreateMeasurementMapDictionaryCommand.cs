using System;
using System.Linq;
using System.Threading.Tasks;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapDictionaryCommands
{
    public class CreateMeasurementMapDictionaryCommand : AsyncCommandBase
    {
        private readonly LoadingStateViewModel _loadingState;
        private readonly MeasurementMapDictionaryStore _store;
        private readonly MeasurementMapDictionaryListViewModel _vm;

        public CreateMeasurementMapDictionaryCommand(
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
            return _vm.NewDictionaryCodeLsf82 > 0
                   && _vm.NewDictionaryFormId > 0
                   && !string.IsNullOrEmpty(_vm.NewDictionaryName);
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            await _loadingState.ExecuteWithLoadingAsync(async () =>
            {
                if (_vm.SelectedRootItems.Count == 0)
                    throw new InvalidOperationException("Для создания карты измерения необходимо указать головные изделия, к которым она будет относиться.");

                var dict = new MeasurementMapDictionary
                {
                    Name = _vm.NewDictionaryName,
                    Code_LSF82 = _vm.NewDictionaryCodeLsf82,
                    MeasurementsMapTypeFormId = _vm.NewDictionaryFormId,
                    Comment = _vm.NewDictionaryComment
                };

                var selectedRootItems = _vm.SelectedRootItems
                    .Select(x => new RootItem() {Id = x.Id, Izdel = x.Name})
                    .ToList();

                if (_vm.CopyMeasurementMapDictionaryId.HasValue)
                    await _store.CopyAsync(_vm.CopyMeasurementMapDictionaryId.Value, dict, selectedRootItems, _vm.User?.Name);
                else
                    await _store.CreateAsync(dict, selectedRootItems, _vm.User?.Name);

                _vm.ClearNewDictionaryFields();

                await _store.LoadAllAsync();
            });
        }
    }
}