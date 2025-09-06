using System.Globalization;
using System.Text.RegularExpressions;
using DefectListDomain.Dtos;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;
using DefectListWpfControl.DefectList.Views;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class SelectItemViewModel : ViewModel
    {
        private readonly ProductsStore _productsStore;
        public bool IsVisibleQty { get; }

        public SelectItemViewModel(ProductsStore productsStore, bool isVisibleQty = true)
        {
            _productsStore = productsStore;
            IsVisibleQty = isVisibleQty;

            QtyMnf = 1;
            QtyMnfString = "1";
        }

        #region Properties
        private string _detal;
        public string Detal
        {
            get { return _detal; }
            set
            {
                _detal = value;
                NotifyPropertyChanged(nameof(Detal));
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        private float _qtyMnf;
        public float QtyMnf
        {
            get { return _qtyMnf; }
            set
            {
                _qtyMnf = value;
                NotifyPropertyChanged(nameof(QtyMnf));
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        private string _qtyMnfString;
        public string QtyMnfString
        {
            get { return _qtyMnfString; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _qtyMnfString = "0";
                    NotifyPropertyChanged(nameof(QtyMnfString));
                    QtyMnf = 0;
                    return;
                }

                var regex = new Regex(@"^(\d+([.,]\d*)?|[.,]\d+)$");
                var isParse = regex.IsMatch(value);

                if (isParse)
                {
                    _qtyMnfString = value;

                    float result;
                    float.TryParse(_qtyMnfString.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out result);
                    QtyMnf = result;

                    NotifyPropertyChanged(nameof(QtyMnfString));
                }
            }
        }

        public bool CanSubmit => !string.IsNullOrWhiteSpace(Detal) && QtyMnf > 0;

        public ProductDto Product { get; private set; }
        #endregion

        #region ConfirmCommand
        public DelegateCommand ConfirmCommand
        {
            get
            {
                return new DelegateCommand(p => Confirm());
            }
        }

        public delegate void NewBomItemSelect(BomItem bomItem, ProductDto product);
        public event NewBomItemSelect NewBomItemSelected;

        private void Confirm()
        {
            NewBomItemSelected?.Invoke(new BomItem() { Detal = _detal, QtyMnf = _qtyMnf}, Product);
        }
        #endregion

        #region OpenChoiceProductWindowCommand
        public DelegateCommand OpenChoiceProductWindowCommand
        {
            get
            {
                return new DelegateCommand(p => OpenChoiceProductWindow());
            }
        }

        private void OpenChoiceProductWindow()
        {
            var choiceProductVm = ChoiceProductViewModel.LoadViewModel(_productsStore);
            var choiceProductWindow = new ChoiceProductWindow(choiceProductVm);

            choiceProductVm.ProductSelected += product =>
            {
                var selectedProduct = choiceProductVm.SelectedProduct;
                Product = selectedProduct?.Product;
                Detal = selectedProduct?.Product.Name ?? Detal;
                NotifyPropertyChanged(nameof(Detal));
                choiceProductWindow.Close();
            };

            choiceProductWindow.ShowDialog();
        }
        #endregion
    }
}