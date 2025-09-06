using System.Windows.Input;
using DefectListDomain.Dtos;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class ProductItemViewModel
    {
        public ProductDto Product { get; }
        public ICommand ProductDoubleClickCommand { get; }

        public ProductItemViewModel(ProductDto product, ICommand productDoubleClickCommand)
        {
            Product = product;
            ProductDoubleClickCommand = productDoubleClickCommand;
        }
    }
}