using System.Windows;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.Commands.BomHeaderCommands
{
    public class OpenEditBomHeaderFormCommand : CommandBase
    {
        private readonly ProductsStore _productsStore;
        private readonly BomHeadersStore _bomHeadersStore;
        private readonly SelectedBomHeaderStore _selectedBomHeaderStore;
        private readonly RootItemsStore _rootItemsStore;
        private readonly CustomIdentity _user;

        public OpenEditBomHeaderFormCommand(ProductsStore productsStore, BomHeadersStore bomHeadersStore, SelectedBomHeaderStore selectedBomHeaderStore, RootItemsStore rootItemsStore, CustomIdentity user)
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

            if (_selectedBomHeaderStore.SelectedBomHeader.StateInfo.IsClosed)
            {
                MessageBox.Show("Дефектовочная ведомость находится в состоянии Закрыто. Выполнение операции запрещено");
                return;
            }

            var vm = new EditBomHeaderViewModel(_productsStore, _selectedBomHeaderStore.SelectedBomHeader, _bomHeadersStore, _rootItemsStore, _user);
            var window = new EditBomHeaderView { DataContext = vm };
            _bomHeadersStore.BomHeaderUpdated += bomHeader => window.Close();
            window.ShowDialog();
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) && PermissionsStore.IsSuperUser;
        }
    }
}