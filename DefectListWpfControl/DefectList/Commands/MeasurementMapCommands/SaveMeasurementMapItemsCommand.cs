using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DefectListDomain.Exceptions;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapCommands
{
    public class SaveMeasurementMapItemsCommand : AsyncCommandBase
    {
        private readonly MeasurementMapViewModel _measurementMapViewModel;
        private readonly MeasurementMapStore _measurementMapStore;
        private readonly DefectListItemViewModel _defectListItemViewModel;

        public SaveMeasurementMapItemsCommand(MeasurementMapViewModel measurementMapViewModel,
            MeasurementMapStore measurementMapStore, DefectListItemViewModel defectListItemViewModel)
        {
            _measurementMapViewModel = measurementMapViewModel;
            _measurementMapStore = measurementMapStore;
            _defectListItemViewModel = defectListItemViewModel;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            await _measurementMapViewModel.LoadingStateViewModel.ExecuteWithLoadingAsync(async () =>
            {
                try
                {
                    var changesVms = _measurementMapViewModel.ItemsView
                        .OfType<MeasurementMapItemViewModel>()
                        .Where(vm => vm.HasChanges)
                        .ToList();

                    var defectDeltas = changesVms
                        .Where(vm => vm.DefectStateChanged)
                        .Select(vm => new DefectDelta
                        {
                            PossibleDefectName = vm.PossibleDefectName,
                            IsPresent = vm.DefectIsNowPresent
                        })
                        .ToList();

                    var models = changesVms
                        .Select(vm => vm.ToModel())
                        .ToList();

                    await _measurementMapStore.SaveItemsAsync(models, _measurementMapViewModel.User?.Name);

                    // Обновление bomItem, чтобы взять актуальные значения из бд, а не кэш во ViewModel.
                    // Если идет редактирование карты измерения двумя пользователями, то необходимо это обновление, 
                    // чтобы не было конфликта при записи в BomItem.Defect
                    await _defectListItemViewModel.LoadBomItemCommand.ExecuteAsync();

                    var bomItem = _defectListItemViewModel.SelectedBomItemViewModel;
                    var isSerialNumberChanged = bomItem != null && bomItem.SerialNumber != _measurementMapViewModel.SerialNumber?.Trim();

                    if (isSerialNumberChanged)
                        bomItem.SerialNumber = _measurementMapViewModel.SerialNumber;

                    if (defectDeltas.Count > 0)
                        ApplyDefectDeltasToBomItem(defectDeltas);

                    if (defectDeltas.Count > 0 || isSerialNumberChanged)
                        await _defectListItemViewModel.SaveDefectPropsCommand.ExecuteAsync();
                }
                catch (ConcurrencyConflictException e)
                {
                    MessageBox.Show(e.Message);
                    await _measurementMapStore.LoadAsync(_defectListItemViewModel.SelectedBomItemViewModel.Id);
                }
            });
        }

        private void ApplyDefectDeltasToBomItem(IReadOnlyList<DefectDelta> deltas)
        {
            var bomItem = _defectListItemViewModel.SelectedBomItemViewModel;
            if (bomItem == null)
                return;

            foreach (var delta in deltas)
            {
                bomItem.Defect = delta.IsPresent
                    ? AddDefectName(bomItem.Defect, delta.PossibleDefectName)
                    : RemoveDefectName(bomItem.Defect, delta.PossibleDefectName);
                bomItem.Decision = "ремонт";
            }
        }

        private string RemoveDefectName(string currentBomItemDefect, string possibleDefectName)
        {
            if (string.IsNullOrEmpty(currentBomItemDefect))
                return currentBomItemDefect;
            if (currentBomItemDefect == possibleDefectName)
                return null;
            if (currentBomItemDefect.StartsWith(possibleDefectName))
                return currentBomItemDefect.Remove(0, possibleDefectName.Length + 2);

            int pos = currentBomItemDefect.IndexOf(possibleDefectName, StringComparison.Ordinal);
            return pos >= 2
                ? currentBomItemDefect.Remove(pos - 2, possibleDefectName.Length + 2)
                : currentBomItemDefect;
        }

        private string AddDefectName(string currentBomItemDefect, string possibleDefectName) =>
            string.IsNullOrEmpty(currentBomItemDefect)
                ? possibleDefectName
                : $"{currentBomItemDefect}, {possibleDefectName}";

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) &&
                   PermissionsStore.IsSuperUser &&
                   _measurementMapViewModel.IsEditable;
        }

        class DefectDelta
        {
            public string PossibleDefectName { get; set; }
            public bool IsPresent { get; set; }
        }
    }
}