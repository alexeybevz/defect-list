using System.Linq;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class OpenSplitBomItemFormCommand : CommandBase
    {
        private readonly DefectListItemViewModel _defectListItemViewModel;
        private readonly SelectedBomItemStore _selectedBomItemStore;
        private readonly BomItemsStore _bomItemsStore;

        public OpenSplitBomItemFormCommand(DefectListItemViewModel defectListItemViewModel, SelectedBomItemStore selectedBomItemStore, BomItemsStore bomItemsStore)
        {
            _defectListItemViewModel = defectListItemViewModel;
            _selectedBomItemStore = selectedBomItemStore;
            _bomItemsStore = bomItemsStore;
        }

        public override void Execute(object parameter = null)
        {
            var parentBomItem = _defectListItemViewModel.BomItemsView.OfType<BomItemViewModel>()
                .FirstOrDefault(x => x.Id == _selectedBomItemStore.SelectedBomItem.ParentId)
                ?.BomItem;

            var vm = new SplitBomItemViewModel((decimal)_selectedBomItemStore.SelectedBomItem.QtyMnf);
            var form = new SplitBomItemWindow() { DataContext = vm };
            vm.BomItemSplitted += (currentBomItemQty, newBomItemQty) =>
            {
                if (newBomItemQty == 0)
                {
                    form.Close();
                    return;
                }

                _bomItemsStore.Split(_defectListItemViewModel.BomHeader, parentBomItem, _selectedBomItemStore.SelectedBomItem, currentBomItemQty, newBomItemQty, _defectListItemViewModel.UserIdentity.Name);
                form.Close();
            };

            form.ShowDialog();
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) &&
                   PermissionsStore.IsSuperUser &&
                   (_defectListItemViewModel.BomHeader.StateInfo.IsWip || _defectListItemViewModel.BomHeader.StateInfo.IsWaitApproved);
        }
    }
}