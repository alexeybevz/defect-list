using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.RootItemCommands
{
    public class ChoiceRootItemInitialCommand : AsyncCommandBase
    {
        private readonly AddRootItemViewModel _addRootItemViewModel;
        private readonly ProductsStore _productsStore;

        public ChoiceRootItemInitialCommand(AddRootItemViewModel addRootItemViewModel,
            ProductsStore productsStore)
        {
            _addRootItemViewModel = addRootItemViewModel;
            _productsStore = productsStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            var choiceProductViewModel = ChoiceProductViewModel.LoadViewModel(_productsStore);
            var choiceProductWindow = new ChoiceProductWindow(choiceProductViewModel);
            choiceProductWindow.Show();

            choiceProductViewModel.ProductSelected += product =>
            {
                var selectedProduct = choiceProductViewModel.SelectedProduct;

                _addRootItemViewModel.ProductInitial = selectedProduct?.Product;
                choiceProductWindow.Close();
            };
        }
    }
}