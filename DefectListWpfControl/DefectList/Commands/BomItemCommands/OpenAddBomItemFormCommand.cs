using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class OpenAddBomItemFormCommand : CommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;
        private readonly SelectedBomItemStore _selectedBomItemStore;
        private readonly ProductsStore _productsStore;
        private readonly BomItemsStore _bomItemsStore;

        public OpenAddBomItemFormCommand(DefectListItemViewModel bomItemViewModel, SelectedBomItemStore selectedBomItemStore, ProductsStore productsStore, BomItemsStore bomItemsStore)
        {
            _bomItemViewModel = bomItemViewModel;
            _selectedBomItemStore = selectedBomItemStore;
            _productsStore = productsStore;
            _bomItemsStore = bomItemsStore;
        }

        public override void Execute(object parameter = null)
        {
            var selectNewItemWindow = new SelectItemWindow(_productsStore);
            var viewModel = ((SelectItemViewModel)selectNewItemWindow.DataContext);
            viewModel.NewBomItemSelected += (newBomItem, product) =>
            {
                _bomItemsStore.Add(_bomItemViewModel.BomHeader, _selectedBomItemStore.SelectedBomItem, newBomItem, _bomItemViewModel.UserIdentity.Name);
            };

            selectNewItemWindow.ShowDialog();
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) && PermissionsStore.IsSuperUser && (_bomItemViewModel.BomHeader.StateInfo.IsWip || _bomItemViewModel.BomHeader.StateInfo.IsWaitApproved);
        }
    }
}