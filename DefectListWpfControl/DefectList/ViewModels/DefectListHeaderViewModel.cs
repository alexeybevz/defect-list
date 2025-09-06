using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Commands.BomHeaderCommands;
using DefectListWpfControl.DefectList.Commands.ReportCommands;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class DefectListHeaderViewModel : ViewModel
    {
        private bool _isVisibleAction;
        public bool IsVisibleAction
        {
            get
            {
                return _isVisibleAction;
            }
            set
            {
                _isVisibleAction = value;
                NotifyPropertyChanged("IsVisibleAction");
            }
        }

        private string _filterString;
        public string FilterString
        {
            get { return _filterString; }
            set
            {
                _filterString = value;
                NotifyPropertyChanged(nameof(FilterString));
                _bomHeadersView.Refresh();
            }
        }

        private string _filterModeName = "Режим: Стандартный фильтр";
        public string FilterModeName
        {
            get { return _filterModeName; }
            set
            {
                _filterModeName = value;
                NotifyPropertyChanged(nameof(FilterModeName));
            }
        }

        public CheckBoxViewModel ShowCompletedHeadersViewModel { get; }

        private string _showedRowsString;
        public string ShowedRowsString
        {
            get
            {
                return _showedRowsString;
            }
            set
            {
                _showedRowsString = value;
                NotifyPropertyChanged("ShowedRowsString");
            }
        }

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

        public BomHeaderViewModel SelectedBomHeaderViewModel
        {
            get
            {
                return _bomHeaderViewModels.FirstOrDefault(x =>
                    x.BomId == _selectedBomHeaderStore.SelectedBomHeader?.BomId);
            }
            set
            {
                _selectedBomHeaderStore.SelectedBomHeader = value?.BomHeader;
                NotifyPropertyChanged(nameof(SelectedBomHeaderViewModel));
            }
        }

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);
        public bool IsCanExecuteSuperUserProperty => PermissionsStore.IsSuperUser;

        private readonly BomHeaderSubscribersStore _bomHeaderSubscribersStore;
        private readonly OpenedBomHeadersStore _openedBomHeadersStore;
        private readonly BomHeadersStore _bomHeadersStore;
        private readonly SelectedBomHeaderStore _selectedBomHeaderStore;
        private readonly ObservableCollection<BomHeaderViewModel> _bomHeaderViewModels;

        private ICollectionView _bomHeadersView;
        public ICollectionView BomHeadersView => _bomHeadersView;

        public ICommand SummaryByOrdersReportCommand { get; }
        public ICommand ChangesFinalDecisionReportCommand { get; }
        public ICommand GetItemInfoByAllProductsReportCommand { get; }
        public ICommand ClearBomHeadersFilterCommand { get; }
        public ICommand LoadBomHeadersCommand { get; }
        private ICommand LoadBomHeaderSubscribesCommand { get; }
        public ICommand ApproveBomHeaderCommand { get; }
        public ICommand OpenAddBomHeaderFormCommand { get; }
        public ICommand OpenEditBomHeaderFormCommand { get; }
        public ICommand OpenAddBomHeaderFormBasedOnSelectedItemCommand { get; }
        public ICommand DeleteBomHeaderCommand { get; }
        public ICommand ExecuteDoubleClickOnBomHeaderCommand { get; }
        public ICommand FilterSettingsCommand { get; }
        public ICommand LoadBomItemsToBomHeaderCommand { get; }

        private DefectListHeaderViewModel(
            BomHeadersStore bomHeadersStore,
            BomItemsStore bomItemsStore,
            SelectedBomHeaderStore selectedBomHeaderStore,
            BomHeaderSubscribersStore bomHeaderSubscribersStore,
            RootItemsStore rootItemsStore,
            OpenedBomHeadersStore openedBomHeadersStore,
            ProductsStore productsStore)
        {
            _bomHeaderSubscribersStore = bomHeaderSubscribersStore;
            _openedBomHeadersStore = openedBomHeadersStore;
            _bomHeadersStore = bomHeadersStore;
            _selectedBomHeaderStore = selectedBomHeaderStore;

            _bomHeaderViewModels = new ObservableCollection<BomHeaderViewModel>();
            _bomHeadersView = CollectionViewSource.GetDefaultView(_bomHeaderViewModels);

            var user = Thread.CurrentPrincipal.Identity as CustomIdentity;
            SummaryByOrdersReportCommand = new SummaryByOrdersReportCommand(bomHeadersStore, bomItemsStore, productsStore, user.Name);
            ChangesFinalDecisionReportCommand = new ChangesFinalDecisionReportCommand(bomItemsStore, user.Name);
            GetItemInfoByAllProductsReportCommand = new GetItemInfoByAllProductsReportCommand(bomHeadersStore, bomItemsStore, productsStore, user.Name);

            LoadBomHeadersCommand = new LoadBomHeadersCommand(this, _bomHeadersStore);
            LoadBomHeaderSubscribesCommand = new LoadBomHeaderSubscribesCommand(_bomHeaderSubscribersStore);
            LoadBomItemsToBomHeaderCommand = new LoadBomItemsToBomHeaderCommand(_bomHeadersStore, _selectedBomHeaderStore, user.Name);

            ClearBomHeadersFilterCommand = new DelegateCommand(p => FilterString = string.Empty);
            ApproveBomHeaderCommand= new UpdateStateFromClosedToApprovedBomHeaderCommand(_selectedBomHeaderStore, _bomHeadersStore, user.Name, BomHeaderState.Approved);
            OpenAddBomHeaderFormCommand = new OpenAddBomHeaderFormCommand(productsStore, _bomHeadersStore, rootItemsStore, user);
            OpenEditBomHeaderFormCommand = new OpenEditBomHeaderFormCommand(productsStore, _bomHeadersStore, _selectedBomHeaderStore, rootItemsStore, user);
            OpenAddBomHeaderFormBasedOnSelectedItemCommand = new OpenAddBomHeaderFormBasedOnSelectedItemCommand(productsStore, _bomHeadersStore, _selectedBomHeaderStore, rootItemsStore, user);
            DeleteBomHeaderCommand = new DeleteBomHeaderCommand(_selectedBomHeaderStore, _bomHeadersStore);
            ExecuteDoubleClickOnBomHeaderCommand = new ExecuteDoubleClickOnBomHeaderCommand(_openedBomHeadersStore);
            FilterSettingsCommand = new OpenFilterSettingsCommand(this, _bomHeadersStore);

            _selectedBomHeaderStore.SelectedBomHeaderChanged += SelectedBomHeaderStore_SelectedBomHeaderChanged;
            _openedBomHeadersStore.RequestOnLoadBomHeaders += Request_LoadBomHeaders;
            _bomHeadersStore.BomHeadersLoaded += BomHeadersStore_BomHeadersLoaded;
            _bomHeadersStore.BomHeaderAdded += BomHeadersStore_BomHeaderAdded;
            _bomHeadersStore.BomHeaderUpdated += BomHeadersStore_BomHeaderUpdated;
            _bomHeadersStore.BomHeaderStateUpdated += BomHeadersStore_BomHeaderStateUpdated;
            _bomHeadersStore.BomHeaderLoadedById += BomHeadersStore_BomHeaderUpdated;
            _bomHeadersStore.BomHeaderDeleted += BomHeadersStore_BomHeaderDeleted;
            _bomHeadersView.CollectionChanged += BomHeadersView_CollectionChanged;

            IsVisibleAction = !PermissionsStore.IsPassiveUser;
            ShowCompletedHeadersViewModel = new CheckBoxViewModel("Показать завершенные ведомости");
            ShowCompletedHeadersViewModel.IsCheckedChanged += RefreshCollectionView;
        }

        public static DefectListHeaderViewModel LoadViewModel(
            BomHeadersStore bomHeadersStore,
            BomItemsStore bomItemsStore,
            SelectedBomHeaderStore selectedBomHeaderStore,
            BomHeaderSubscribersStore bomHeaderSubscribersStore,
            RootItemsStore rootItemsStore,
            OpenedBomHeadersStore openedBomHeadersStore,
            ProductsStore productsStore)
        {
            var viewModel = new DefectListHeaderViewModel(
                bomHeadersStore,
                bomItemsStore,
                selectedBomHeaderStore,
                bomHeaderSubscribersStore,
                rootItemsStore,
                openedBomHeadersStore,
                productsStore);
            viewModel.LoadBomHeadersCommand.Execute(null);
            return viewModel;
        }

        protected override void Dispose()
        {
            _selectedBomHeaderStore.SelectedBomHeaderChanged -= SelectedBomHeaderStore_SelectedBomHeaderChanged;
            _openedBomHeadersStore.RequestOnLoadBomHeaders -= Request_LoadBomHeaders;
            _bomHeadersStore.BomHeadersLoaded -= BomHeadersStore_BomHeadersLoaded;
            _bomHeadersStore.BomHeaderAdded -= BomHeadersStore_BomHeaderAdded;
            _bomHeadersStore.BomHeaderUpdated -= BomHeadersStore_BomHeaderUpdated;
            _bomHeadersStore.BomHeaderStateUpdated -= BomHeadersStore_BomHeaderStateUpdated;
            _bomHeadersStore.BomHeaderLoadedById -= BomHeadersStore_BomHeaderUpdated;
            _bomHeadersStore.BomHeaderDeleted -= BomHeadersStore_BomHeaderDeleted;
            _bomHeadersView.CollectionChanged -= BomHeadersView_CollectionChanged;
            ShowCompletedHeadersViewModel.IsCheckedChanged -= RefreshCollectionView;

            base.Dispose();
        }

        private void SelectedBomHeaderStore_SelectedBomHeaderChanged()
        {
            NotifyPropertyChanged(nameof(SelectedBomHeaderViewModel));
        }

        private void BomHeadersStore_BomHeadersLoaded()
        {
            var selectedBomHeaderId = SelectedBomHeaderViewModel?.BomId ?? 0;

            // После выполнения строки в SelectedBomHeaderViewModel присваивается null (поведение WPF по умолчанию)
            _bomHeaderViewModels.Clear();

            foreach (var bomHeader in _bomHeadersStore.BomHeaders)
            {
                _bomHeaderViewModels.Add(new BomHeaderViewModel(_bomHeadersStore, _bomHeaderSubscribersStore, _openedBomHeadersStore, bomHeader));
            }

            _bomHeadersView = CollectionViewSource.GetDefaultView(_bomHeaderViewModels);

            LoadBomHeaderSubscribesCommand.Execute(null);

            SelectedBomHeaderViewModel = _bomHeaderViewModels.FirstOrDefault(x =>
                x.BomId == selectedBomHeaderId);
        }

        private void BomHeadersStore_BomHeaderAdded(BomHeader bomHeader)
        {
            _bomHeaderViewModels.Insert(0, new BomHeaderViewModel(_bomHeadersStore, _bomHeaderSubscribersStore, _openedBomHeadersStore, bomHeader));
        }

        private void BomHeadersStore_BomHeaderUpdated(BomHeader bomHeader)
        {
            BomHeaderViewModelUpdate(bomHeader);
        }

        private void BomHeadersStore_BomHeaderStateUpdated(BomHeader bomHeader)
        {
            BomHeaderViewModelUpdate(bomHeader);
        }

        private void BomHeaderViewModelUpdate(BomHeader bomHeader)
        {
            var selectedBomHeaderId = SelectedBomHeaderViewModel?.BomId ?? 0;

            var bomHeaderViewModel = _bomHeaderViewModels.FirstOrDefault(x => x.BomHeader?.BomId == bomHeader.BomId);
            bomHeaderViewModel?.Update(bomHeader);
            NotifyPropertyChanged(nameof(SelectedBomHeaderViewModel));

            if (selectedBomHeaderId == bomHeader.BomId)
            {
                SelectedBomHeaderViewModel = _bomHeaderViewModels.FirstOrDefault(x =>
                    x.BomId == selectedBomHeaderId);
            }
        }

        private void BomHeadersStore_BomHeaderDeleted(int id)
        {
            var bomHeaderViewModel = _bomHeaderViewModels.FirstOrDefault(x => x.BomHeader?.BomId == id);
            if (bomHeaderViewModel != null)
            {
                _bomHeaderViewModels.Remove(bomHeaderViewModel);
            }
        }

        private void BomHeadersView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ShowedRowsString = $"Показано строк: {_bomHeadersView.OfType<BomHeaderViewModel>().Count()} из {_bomHeadersStore.BomHeaders.Count()}";
        }

        private void RefreshCollectionView()
        {
            BomHeadersView.Refresh();
        }

        private void Request_LoadBomHeaders()
        {
            LoadBomHeadersCommand?.Execute(null);
        }
    }
}