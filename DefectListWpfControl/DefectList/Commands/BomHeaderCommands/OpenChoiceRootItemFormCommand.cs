using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomHeaderCommands
{
    public class OpenChoiceRootItemFormCommand : AsyncCommandBase
    {
        private readonly BomHeaderDetailsFormViewModel _bomHeaderDetailsFormViewModel;
        private readonly ProductsStore _productsStore;
        private readonly RootItemsStore _rootItemsStore;

        public OpenChoiceRootItemFormCommand(BomHeaderDetailsFormViewModel bomHeaderDetailsFormViewModel, ProductsStore productsStore, RootItemsStore rootItemsStore)
        {
            _bomHeaderDetailsFormViewModel = bomHeaderDetailsFormViewModel;
            _productsStore = productsStore;
            _rootItemsStore = rootItemsStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            var vm = ChoiceRootItemViewModel.LoadViewModel(_productsStore, _rootItemsStore);
            var window = new ChoiceRootItemWindow() { DataContext = vm };
            vm.RootItemSelected += rootItem =>
            {
                _bomHeaderDetailsFormViewModel.RootItem = rootItem;
                window.Close();
            };
            window.ShowDialog();
        }
    }
}