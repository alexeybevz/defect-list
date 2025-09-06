using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DefectListDomain.Dtos;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands
{
    public class LoadProductsCommand : AsyncCommandBase
    {
        private readonly ChoiceProductViewModel _choiceProductViewModel;
        private readonly ProductsStore _productsStore;
        public LoadProductsCommand(ChoiceProductViewModel choiceProductViewModel, ProductsStore productsStore)
        {
            _choiceProductViewModel = choiceProductViewModel;
            _productsStore = productsStore;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            _choiceProductViewModel.ErrorMessage = null;
            _choiceProductViewModel.IsLoading = true;

            try
            {
                var products = await _productsStore.GetAllDesignSpecifications();
                _choiceProductViewModel.Products = new ObservableCollection<ProductDto>(products);
            }
            catch (Exception ex)
            {
                _choiceProductViewModel.ErrorMessage = $"Возникла ошибка при получении номенклатуры из базы данных: {ex.Message}";
            }
            finally
            {
                _choiceProductViewModel.IsLoading = false;
            }
        }
    }
}