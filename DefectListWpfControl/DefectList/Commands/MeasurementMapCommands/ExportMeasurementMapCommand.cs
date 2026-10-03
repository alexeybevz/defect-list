using System;
using System.Threading;
using System.Threading.Tasks;
using DefectListDomain.CreatingReports;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapCommands
{
    public class ExportMeasurementMapCommand : AsyncCommandBase
    {
        private readonly DefectListItemViewModel _defectListItemViewModel;
        private readonly MeasurementMapStore _measurementMapStore;
        private readonly SelectedBomItemStore _selectedBomItemStore;
        private readonly CustomIdentity _user;

        public ExportMeasurementMapCommand(
            DefectListItemViewModel defectListItemViewModel,
            MeasurementMapStore measurementMapStore,
            SelectedBomItemStore selectedBomItemStore)
        {
            _defectListItemViewModel = defectListItemViewModel;
            _measurementMapStore = measurementMapStore;
            _selectedBomItemStore = selectedBomItemStore;
            _user = Thread.CurrentPrincipal.Identity as CustomIdentity;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            _defectListItemViewModel.MeasurementMapViewModel.LoadingStateViewModel.ErrorMessage = null;

            try
            {
                var report = DefectListIocKernel.Get<IMeasurementMapReport>();
                await report.CreateAsync(
                    _measurementMapStore.MeasurementMap,
                    _selectedBomItemStore.SelectedBomItem,
                    _defectListItemViewModel.BomHeader,
                    _user?.Name);
            }
            catch (Exception e)
            {
                _defectListItemViewModel.MeasurementMapViewModel.LoadingStateViewModel.ErrorMessage = e.Message;
            }
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) &&
                   _measurementMapStore.HasMeasurementMap;
        }
    }
}