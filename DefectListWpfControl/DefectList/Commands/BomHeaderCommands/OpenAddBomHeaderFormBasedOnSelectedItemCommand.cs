using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.Commands.BomHeaderCommands
{
    public class OpenAddBomHeaderFormBasedOnSelectedItemCommand : CommandBase
    {
        private readonly ProductsStore _productsStore;
        private readonly BomHeadersStore _bomHeadersStore;
        private readonly SelectedBomHeaderStore _selectedBomHeaderStore;
        private readonly RootItemsStore _rootItemsStore;
        private readonly CustomIdentity _user;

        public OpenAddBomHeaderFormBasedOnSelectedItemCommand(ProductsStore productsStore, BomHeadersStore bomHeadersStore, SelectedBomHeaderStore selectedBomHeaderStore, RootItemsStore rootItemsStore, CustomIdentity user)
        {
            _productsStore = productsStore;
            _bomHeadersStore = bomHeadersStore;
            _selectedBomHeaderStore = selectedBomHeaderStore;
            _rootItemsStore = rootItemsStore;
            _user = user;
        }

        public override void Execute(object parameter = null)
        {
            if (_selectedBomHeaderStore.SelectedBomHeader == null)
                return;

            var vm = new AddBomHeaderViewModel(_productsStore, _bomHeadersStore, _rootItemsStore, _user, _selectedBomHeaderStore.SelectedBomHeader);
            var window = new AddBomHeaderView { DataContext = vm };
            _bomHeadersStore.BomHeaderAdded += bomHeader => window.Close();
            window.ShowDialog();
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) && PermissionsStore.IsSuperUser;
        }
    }
}