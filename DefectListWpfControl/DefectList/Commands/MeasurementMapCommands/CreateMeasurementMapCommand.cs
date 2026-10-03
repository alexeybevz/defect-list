using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DefectListDomain.Exceptions;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapCommands
{
    public class CreateMeasurementMapCommand : AsyncCommandBase
    {
        private readonly MeasurementMapViewModel _measurementMapViewModel;
        private readonly DefectListItemViewModel _defectListItemViewModel;
        private readonly MeasurementMapStore _measurementMapStore;
        private readonly SelectedBomItemStore _selectedBomItemStore;

        public CreateMeasurementMapCommand(
            MeasurementMapViewModel measurementMapViewModel,
            DefectListItemViewModel defectListItemViewModel,
            MeasurementMapStore measurementMapStore,
            SelectedBomItemStore selectedBomItemStore)
        {
            _measurementMapViewModel = measurementMapViewModel;
            _defectListItemViewModel = defectListItemViewModel;
            _measurementMapStore = measurementMapStore;
            _selectedBomItemStore = selectedBomItemStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            await _measurementMapViewModel.LoadingStateViewModel.ExecuteWithLoadingAsync(async () =>
            {
                try
                {
                    if (_selectedBomItemStore.SelectedBomItem.Code_LSF82 == null)
                        throw new InvalidOperationException(
                            "Для данной ДСЕ не задан код номенклатуры (Code_LSF82). " +
                            "Карту измерений создать невозможно.");

                    var rootItemId = _defectListItemViewModel.BomHeader.RootItem.Id;

                    await _measurementMapStore.CreateAsync(
                        _selectedBomItemStore.SelectedBomItem.Id,
                        _selectedBomItemStore.SelectedBomItem.Code_LSF82.Value,
                        rootItemId,
                        (Thread.CurrentPrincipal.Identity as CustomIdentity)?.Name);
                }
                catch (MeasurementMapAlreadyExistsException e)
                {
                    MessageBox.Show(e.Message);
                    await _measurementMapStore.LoadAsync(_selectedBomItemStore.SelectedBomItem.Id);
                }
            });
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) && 
                   PermissionsStore.IsSuperUser &&
                   !_measurementMapViewModel.HasMap;
        }
    }
}