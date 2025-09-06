using System.Windows.Input;
using DefectListDomain.Dtos;
using DefectListWpfControl.DefectList.Commands.RootItemCommands;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class AddRootItemViewModel : ViewModel
    {
        public ICommand ChoiceRootItemCommand { get; }
        public ICommand ChoiceRootItemInitialCommand { get; }
        public ICommand AddRootItemCommand { get; }

        public AddRootItemViewModel(ProductsStore productsStore, RootItemsStore rootItemsStore)
        {
            ChoiceRootItemCommand = new ChoiceRootItemCommand(this, productsStore);
            ChoiceRootItemInitialCommand = new ChoiceRootItemInitialCommand(this, productsStore);
            AddRootItemCommand = new AddRootItemCommand(this, rootItemsStore);
        }

        private bool _isSubmitting;
        public bool IsSubmitting
        {
            get
            {
                return _isSubmitting;
            }
            set
            {
                _isSubmitting = value;
                NotifyPropertyChanged(nameof(IsSubmitting));
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
                NotifyPropertyChanged(nameof(ErrorMessage));
                NotifyPropertyChanged(nameof(HasErrorMessage));
            }
        }

        private ProductDto _product;
        public ProductDto Product
        {
            get
            {
                return _product;
            }
            set
            {
                _product = value;
                NotifyPropertyChanged(nameof(Product));
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        private ProductDto _productInitial;
        public ProductDto ProductInitial
        {
            get
            {
                return _productInitial;
            }
            set
            {
                _productInitial = value;
                NotifyPropertyChanged(nameof(ProductInitial));
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);
        public bool CanSubmit => Product != null && ProductInitial != null && Product.Name.StartsWith(ProductInitial.Name);
    }
}