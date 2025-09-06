using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using DefectListDomain.Models;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class ChoiceOrdersViewModel : ViewModel
    {
        private ListCollectionView _orders;
        private string _filterString;
        private bool _isSelectAll;

        #region Properties
        public ListCollectionView Orders
        {
            get { return _orders; }
            set
            {
                _orders = value;
                NotifyPropertyChanged("Orders");
            }
        }

        public bool IsSelectAll
        {
            get { return _isSelectAll; }
            set
            {
                _isSelectAll = value;
                NotifyPropertyChanged("IsSelectAll");

                foreach (var order in Orders.OfType<ItemPresenter>())
                    order.IsSelected = _isSelectAll;
            }
        }
        #endregion

        public ChoiceOrdersViewModel(IEnumerable<BomHeader> bomHeaders)
        {
            Orders = new ListCollectionView(bomHeaders.OrderBy(x => x.Orders).Select(x => new ItemPresenter(x)).ToList());
            Orders.Filter = OnOrdersFiltered;
        }

        #region ConfirmSelectionCommand
        public delegate void OrdersSelectedDelegate(IEnumerable<BomHeader> selectedBomHeaders);
        public event OrdersSelectedDelegate OnOrdersSelected;

        public DelegateCommand ConfirmSelectionCommand
        {
            get
            {
                return new DelegateCommand(p => ConfirmSelection());
            }
        }

        private void ConfirmSelection()
        {
            FilterString = "";
            var selectedItems = Orders.OfType<ItemPresenter>().Where(x => x.IsSelected).Select(x => x.Item).ToList();
            OnOrdersSelected?.Invoke(selectedItems);
        }

        #endregion

        #region Filter
        public string FilterString
        {
            get { return _filterString; }
            set
            {
                _filterString = value;
                NotifyPropertyChanged("FilterString");
                Orders.Refresh();
            }
        }

        private bool OnOrdersFiltered(object obj)
        {
            if (string.IsNullOrWhiteSpace(_filterString)) return true;

            var order = ((ItemPresenter)obj).Name;
            if (order != null && order.ToLower().Contains(_filterString.ToLower()))
                return true;

            return false;
        }
        #endregion

        #region ItemPresenter
        public class ItemPresenter : ObservableObject
        {
            private bool _isSelected;

            public ItemPresenter(BomHeader item, bool isSelected = false)
            {
                Item = item;
                _isSelected = isSelected;
            }

            public BomHeader Item { get; }
            public string Name => Item.Orders;

            public bool IsSelected
            {
                get { return _isSelected; }
                set
                {
                    _isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }

            public override string ToString()
            {
                return Item.Orders;
            }
        }
        #endregion
    }
}
