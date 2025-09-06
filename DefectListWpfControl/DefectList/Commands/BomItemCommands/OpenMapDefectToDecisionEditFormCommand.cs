using System;
using System.Threading.Tasks;
using System.Windows;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class OpenMapDefectToDecisionEditFormCommand : AsyncCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;
        private readonly DefectToDecisionMapsStore _defectToDecisionMapsStore;
        private readonly SelectedDefectToDecisionMapStore _selectedDefectToDecisionMapStore;

        public OpenMapDefectToDecisionEditFormCommand(
            DefectListItemViewModel bomItemViewModel,
            DefectToDecisionMapsStore defectToDecisionMapsStore,
            SelectedDefectToDecisionMapStore selectedDefectToDecisionMapStore)
        {
            _bomItemViewModel = bomItemViewModel;
            _defectToDecisionMapsStore = defectToDecisionMapsStore;
            _selectedDefectToDecisionMapStore = selectedDefectToDecisionMapStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            try
            {
                var vm = DefectToDecisionMapsViewModel.LoadViewModel(_defectToDecisionMapsStore, _selectedDefectToDecisionMapStore);
                var form = new MapDefectToDecisionWindow() { DataContext = vm };
                form.ShowDialog();
                await _defectToDecisionMapsStore.Load();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) && PermissionsStore.IsSuperUser;
        }
    }
}