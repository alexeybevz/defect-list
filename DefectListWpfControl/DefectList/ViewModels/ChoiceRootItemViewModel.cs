using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Commands.RootItemCommands;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class ChoiceRootItemViewModel : ViewModel
    {
        private readonly RootItemsStore _rootItemsStore;
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

        private RootItemViewModel _selectedRootItemViewModel;
        public RootItemViewModel SelectedRootItemViewModel
        {
            get
            {
                return _selectedRootItemViewModel;
            }
            set
            {
                _selectedRootItemViewModel = value;

                NotifyPropertyChanged(nameof(SelectedRootItemViewModel));
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        private ObservableCollection<RootItemViewModel> _rootItemViewModels;
        public ObservableCollection<RootItemViewModel> RootItemViewModels
        {
            get { return _rootItemViewModels; }
            set
            {
                _rootItemViewModels = value;

                NotifyPropertyChanged(nameof(RootItemViewModels));

                SelectedRootItemViewModel = null;
                NotifyPropertyChanged(nameof(SelectedRootItemViewModel));
            }
        }

        public bool CanSubmit => SelectedRootItemViewModel != null;

        public ICommand LoadRootItemsCommand { get; }
        public ICommand OpenAddRootItemsFormCommand { get; }
        public ICommand DeleteRootItemsCommand { get; }
        public ICommand RootItemDoubleClickCommand { get; }
        public ICommand ChoiceRootItemCommand { get; }

        public delegate void RootItemSelect(RootItem rootItem);
        public event RootItemSelect RootItemSelected;

        private AddRootItemWindow _addRootItemWindow;

        private ChoiceRootItemViewModel(ProductsStore productsStore, RootItemsStore rootItemsStore)
        {
            _rootItemsStore = rootItemsStore;
            _rootItemViewModels = new ObservableCollection<RootItemViewModel>();

            LoadRootItemsCommand = new LoadRootItemsCommand(this, _rootItemsStore);
            OpenAddRootItemsFormCommand = new DelegateCommand(p => 
            {
                _addRootItemWindow = new AddRootItemWindow();
                var vm = new AddRootItemViewModel(productsStore, _rootItemsStore);
                _addRootItemWindow.DataContext = vm;
                _addRootItemWindow.ShowDialog();
            });
            DeleteRootItemsCommand = new DeleteRootItemCommand(this, _rootItemsStore);

            RootItemDoubleClickCommand = new DelegateCommand(obj =>
            {
                RootItemSelected?.Invoke(SelectedRootItemViewModel.RootItem);
            });
            ChoiceRootItemCommand = RootItemDoubleClickCommand;
            
            _rootItemsStore.RootItemLoaded += RootItemsStoreOnRootItemLoaded;
            _rootItemsStore.RootItemAdded += RootItemsStoreOnRootItemAdded;
            _rootItemsStore.RootItemDeleted += RootItemsStoreOnRootItemDeleted;
        }

        protected override void Dispose()
        {
            _rootItemsStore.RootItemLoaded -= RootItemsStoreOnRootItemLoaded;
            _rootItemsStore.RootItemAdded -= RootItemsStoreOnRootItemAdded;
            _rootItemsStore.RootItemDeleted -= RootItemsStoreOnRootItemDeleted;

            base.Dispose();
        }

        private void RootItemsStoreOnRootItemLoaded()
        {
            RootItemViewModels = new ObservableCollection<RootItemViewModel>(_rootItemsStore.RootItems.Select(x => new RootItemViewModel(x, RootItemDoubleClickCommand)).ToList());
        }

        private void RootItemsStoreOnRootItemAdded(RootItem rootItem)
        {
            _addRootItemWindow?.Close();
            _rootItemViewModels.Insert(0, new RootItemViewModel(rootItem, RootItemDoubleClickCommand));
        }

        private void RootItemsStoreOnRootItemDeleted(int id)
        {
            var rootItemViewModel = _rootItemViewModels.FirstOrDefault(x => x.RootItem?.Id == id);
            if (rootItemViewModel != null)
            {
                _rootItemViewModels.Remove(rootItemViewModel);
            }
        }

        public static ChoiceRootItemViewModel LoadViewModel(ProductsStore productsStore, RootItemsStore rootItemsStore)
        {
            var vm = new ChoiceRootItemViewModel(productsStore, rootItemsStore);
            vm.LoadRootItemsCommand?.Execute(null);
            return vm;
        }
    }
}