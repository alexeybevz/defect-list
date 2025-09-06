using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.Commands.BomHeaderCommands
{
    public class OpenAddBomHeaderFormCommand : AsyncCommandBase
    {
        private readonly ProductsStore _productsStore;
        private readonly BomHeadersStore _bomHeadersStore;
        private readonly RootItemsStore _rootItemsStore;
        private readonly CustomIdentity _user;

        public OpenAddBomHeaderFormCommand(ProductsStore productsStore, BomHeadersStore bomHeadersStore, RootItemsStore rootItemsStore, CustomIdentity user)
        {
            _productsStore = productsStore;
            _bomHeadersStore = bomHeadersStore;
            _rootItemsStore = rootItemsStore;
            _user = user;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            var vm = new AddBomHeaderViewModel(_productsStore, _bomHeadersStore, _rootItemsStore, _user);
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