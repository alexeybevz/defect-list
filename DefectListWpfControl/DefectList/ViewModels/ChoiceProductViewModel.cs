using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using DefectListDomain.Dtos;
using DefectListWpfControl.DefectList.Commands;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;
using ReporterBusinessLogic;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class ChoiceProductViewModel : ViewModel
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged(nameof(IsLoading));
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

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        private ProductItemViewModel _selectedProduct;
        public ProductItemViewModel SelectedProduct
        {
            get
            {
                return _selectedProduct;
            }
            set
            {
                _selectedProduct = value;

                NotifyPropertyChanged(nameof(SelectedProduct));
                NotifyPropertyChanged(nameof(SelectedProductText));
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        private ObservableCollection<ProductDto> _products;
        public ObservableCollection<ProductDto> Products
        {
            get { return _products; }
            set
            {
                _products = value;

                _productsView = CollectionViewSource.GetDefaultView(_products.Select(x => new ProductItemViewModel(x, ProductDoubleClickCommand)).ToList());
                _productsView.Filter = FilterProducts;

                NotifyPropertyChanged(nameof(Products));
                NotifyPropertyChanged(nameof(ProductsView));

                SelectedProduct = null;
                NotifyPropertyChanged(nameof(SelectedProduct));
            }
        }

        private ICollectionView _productsView;
        public ICollectionView ProductsView => _productsView;

        private string _filterText;
        public string FilterText
        {
            get
            {
                return _filterText;
            }
            set
            {
                _filterText = value;
                NotifyPropertyChanged(nameof(FilterText));

                _productsView?.Refresh();

                FilterTextUpdated?.Invoke();
            }
        }

        public string SelectedProductText => $"Выбранное ДСЕ: {SelectedProduct?.Product.Name}";

        public bool CanSubmit => SelectedProduct != null;

        public ICommand LoadProductsCommand { get; }
        public ICommand ProductDoubleClickCommand { get; }
        public ICommand ChoiceProductCommand { get; }

        public delegate void ProductSelect(ProductDto product);
        public event ProductSelect ProductSelected;

        public delegate void FilterTextUpdate();
        public event FilterTextUpdate FilterTextUpdated;

        private ChoiceProductViewModel(ProductsStore productsStore)
        {
            LoadProductsCommand = new LoadProductsCommand(this, productsStore);
            ProductDoubleClickCommand = new DelegateCommand(obj =>
            {
                ProductSelected?.Invoke(SelectedProduct.Product);
            });
            ChoiceProductCommand = ProductDoubleClickCommand;
        }

        public static ChoiceProductViewModel LoadViewModel(ProductsStore productsStore)
        {
            var vm = new ChoiceProductViewModel(productsStore);
            vm.LoadProductsCommand?.Execute(null);
            return vm;
        }

        private bool FilterProducts(object obj)
        {
            var product = ((ProductItemViewModel)obj).Product;
            if (string.IsNullOrWhiteSpace(FilterText))
                return true; // No filter, show all

            // Filter by Name (Обозначение ДСЕ), case-insensitive
            if (product.Name.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            // Filter by Detals (Код ДСЕ в АСУП), case-insensitive
            if (SpecifKeyCreator.CreateKey(product.Name).IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            return false;
        }
    }
}