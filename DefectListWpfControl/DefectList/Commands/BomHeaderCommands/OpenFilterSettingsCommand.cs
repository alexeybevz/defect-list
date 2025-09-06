using System;
using System.Windows;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;
using DefectListWpfControl.DefectList.Stores;

namespace DefectListWpfControl.DefectList.Commands.BomHeaderCommands
{
    public class OpenFilterSettingsCommand : CommandBase
    {
        private readonly DefectListHeaderViewModel _defectListHeaderViewModel;
        private readonly DefectListHeaderFilterModeWindow _filterModeWindow;

        public OpenFilterSettingsCommand(DefectListHeaderViewModel defectListHeaderViewModel, BomHeadersStore bomHeadersStore)
        {
            _defectListHeaderViewModel = defectListHeaderViewModel;
            
            _filterModeWindow = new DefectListHeaderFilterModeWindow(new DefectListHeaderFilterModeViewModel(defectListHeaderViewModel, bomHeadersStore));

            bomHeadersStore.BomHeadersLoaded += SubscribeOnFilterEvent;
        }

        public override void Execute(object parameter = null)
        {
            try
            {
                _filterModeWindow.ShowDialog();

                SubscribeOnFilterEvent();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void SetFilter(DefectListHeaderFilterModeViewModel vm)
        {
            var filterMode = vm.IsSelectedFilterMode;

            _defectListHeaderViewModel.FilterModeName = $"Режим: {filterMode.Text}";
            _defectListHeaderViewModel.BomHeadersView.Filter = filterMode.Filter;
        }

        private void SubscribeOnFilterEvent()
        {
            var vm = (DefectListHeaderFilterModeViewModel)_filterModeWindow.DataContext;
            SetFilter(vm);
        }
    }
}