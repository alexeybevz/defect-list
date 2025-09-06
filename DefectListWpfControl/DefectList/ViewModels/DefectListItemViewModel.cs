using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DefectListBusinessLogic;
using DefectListDomain.Commands;
using DefectListDomain.ExternalData;
using DefectListDomain.Models;
using DefectListDomain.Services;
using DefectListWpfControl.DefectList.Commands.BomItemCommands;
using DefectListWpfControl.DefectList.Commands.ReportCommands;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class DefectListItemViewModel : ViewModel
    {
        private Message _message;
        private readonly Message _defaultMessage = new DefaultMessage();
        public readonly Message _successMessage = new SuccessMessage();
        public readonly Message _errorMessage = new ErrorMessage();
       
        private readonly BomHeadersStore _bomHeadersStore;
        private readonly SelectedBomItemStore _selectedBomItemStore;
        private readonly BomItemsStore _bomItemsStore;
        private readonly DefectToDecisionMapsStore _defectToDecisionMapsStore;
        private readonly TehprocHeadersStore _tehprocHeadersStore;

        #region MainProperties
        public string Title => $"Деф. вед. № {BomHeader.BomId} / {BomHeader.SerialNumber} / {BomHeader.RootItem.Izdel} / {BomHeader.Contract}";

        private readonly ObservableCollection<BomItemViewModel> _bomItemViewModels;
        private ICollectionView _bomItemsView;
        public ICollectionView BomItemsView => _bomItemsView;
        public ObservableCollection<BomItemLog> BomItemLogs => _selectedBomItemStore?.BomItemLogs;
        public ObservableCollection<MapBomItemToRouteChart> MapBomItemToRouteCharts => _selectedBomItemStore?.MapBomItemToRouteCharts;

        public BomItemViewModel SelectedBomItemViewModel
        {
            get
            {
                return _bomItemViewModels.FirstOrDefault(x =>
                    x.Id == _selectedBomItemStore.SelectedBomItem?.Id);
            }
            set
            {
                _selectedBomItemStore.SelectedBomItem = value?.BomItem;
                NotifyPropertyChanged(nameof(SelectedBomItemViewModel));
            }
        }

        public BomHeader BomHeader { get; set; }
        public CustomIdentity UserIdentity { get; }
        public Dictionary<string, FieldPermission> FieldsPermissions { get; set; }

        private ICollectionView _defectToDecisionMaps;
        public ICollectionView DefectToDecisionMaps
        {
            get { return _defectToDecisionMaps; }
            set
            {
                _defectToDecisionMaps = value;
                NotifyPropertyChanged(nameof(DefectToDecisionMaps));
            }
        }

        private string _selectedPossibleDefect;
        public string SelectedPossibleDefect
        {
            get { return _selectedPossibleDefect; }
            set
            {
                if (value == null)
                {
                    _selectedPossibleDefect = value;
                    NotifyPropertyChanged(nameof(SelectedPossibleDefect));
                    return;
                }

                DefectToDecisionMaps?.OfType<DefectToDecisionMapCheckBoxViewModel>().Where(x => x.IsSelected).ToList().ForEach(x => { x.IsSelected = false; });

                _selectedPossibleDefect = value;

                var obj = DefectToDecisionMaps?.OfType<DefectToDecisionMapCheckBoxViewModel>()
                    .FirstOrDefault(x => x.Item.Defect == value);

                if (obj != null)
                    obj.IsSelected = true;

                NotifyPropertyChanged(nameof(SelectedPossibleDefect));
            }
        }
        #endregion

        #region Properties

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

        private string _loadingErrorMessage;
        public string LoadingErrorMessage
        {
            get
            {
                return _loadingErrorMessage;
            }
            set
            {
                _loadingErrorMessage = value;
                NotifyPropertyChanged(nameof(LoadingErrorMessage));
                NotifyPropertyChanged(nameof(HasErrorLoadingMessage));
            }
        }
        public bool HasErrorLoadingMessage => !string.IsNullOrEmpty(LoadingErrorMessage);

        private string _filterString;
        public string FilterString
        {
            get { return _filterString; }
            set
            {
                _filterString = value;
                NotifyPropertyChanged(nameof(FilterString));
                BomItemsView.Refresh();
            }
        }

        private List<BomItem> _searchedBomItemParents = new List<BomItem>();
        private string _searchString;
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                if (_searchString == value)
                    return;

                _searchString = value;

                _filterString = string.Empty; // Необходимо для поиска, чтобы пользователь увидел состав сб.ед, в которые входит искомое ДСЕ
                NotifyPropertyChanged(nameof(FilterString));

                NotifyPropertyChanged(nameof(SearchString));

                var searchedBomItemParentIds = _bomItemsStore.BomItems.Where(x => x.Detal == _searchString).Select(x => x.ParentId).Distinct().ToList();
                _searchedBomItemParents = _bomItemsStore.BomItems.Where(x => searchedBomItemParentIds.Contains(x.Id)).ToList();

                BomItemsView.Refresh();
            }
        }

        private bool _isCanEditBomItem;
        public bool IsCanEditBomItem
        {
            get
            {
                return _isCanEditBomItem;
            }
            set
            {
                _isCanEditBomItem = value;
                NotifyPropertyChanged(nameof(IsCanEditBomItem));
            }
        }

        private decimal _fillPercentage;
        public decimal FillPercentage
        {
            get { return _fillPercentage; }
            set
            {
                _fillPercentage = value;
                NotifyPropertyChanged(nameof(FillPercentage));
            }
        }

        private string _fillText;
        public string FillText
        {
            get { return _fillText; }
            set
            {
                _fillText = value;
                NotifyPropertyChanged(nameof(FillText));
            }
        }

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
                NotifyPropertyChanged(nameof(IsVisibleAction));
            }
        }

        private bool _isCanExecuteAction;
        public bool IsCanExecuteAction
        {
            get
            {
                return _isCanExecuteAction;
            }
            set
            {
                _isCanExecuteAction = value;
                NotifyPropertyChanged(nameof(IsCanExecuteAction));
            }
        }

        private bool _isCanEditBomAction;
        public bool IsCanEditBomAction
        {
            get
            {
                return _isCanEditBomAction;
            }
            set
            {
                _isCanEditBomAction = value;
                NotifyPropertyChanged(nameof(IsCanEditBomAction));
            }
        }

        public ObservableCollection<string> Detals { get; }

        public ObservableCollection<string> PossibleDecisions { get; }

        public List<string> IsRequiredSubmitLabels => BomItemsConstantsStore.IsRequiredSubmitDict.Values.Select(x => x).ToList();
        public List<string> IsSubmittedLabels => BomItemsConstantsStore.IsSubmittedDict.Values.Select(x => x).ToList();
        public List<string> FilteredColumns => BomItemsConstantsStore.FilteredColumns.Keys.Select(x => x).ToList();

        private string _selectedFilteredColumn;
        public string SelectedFilteredColumn
        {
            get { return _selectedFilteredColumn; }
            set
            {
                _selectedFilteredColumn = value;
                FilterBomItemCommand?.Execute(null);
                NotifyPropertyChanged(nameof(SelectedFilteredColumn));
            }
        }
        #endregion

        #region ViewModels
        public BomItemsFilterByDetalTypViewModel BomItemsFilterByDetalTypViewModel { get; }
        #endregion

        #region Commands

        public ICommand ExportDefectListWithFinalDecisionToPdfCommand { get; }
        public ICommand ExportDefectListWithInitialDecisionToPdfCommand { get; }
        public ICommand ExportAssemblyWithFinalDecisionToPdfCommand { get; }
        public ICommand ExportAssemblyWithInitialDecisionToPdfCommand { get; }
        public ICommand ExportDefectListToExcelCommand { get; }
        public ICommand ExportDefectListReclamationToPdfCommand { get; }
        public ICommand ExportAssemblyReclamationToPdfCommand { get; }
        public ICommand PurchaseItemsReportCommand { get; }
        public ICommand ScrapItemsReportCommand { get; }
        public ICommand DefectListItemsChangesReportCommand { get; }
        public ICommand AdditionalMaterialsReportCommand { get; }

        public ICommand OpenAddBomItemFormCommand { get; }
        public ICommand OpenReplaceBomItemFormCommand { get; }
        public ICommand OpenReplaceNameBomItemFormCommand { get; }
        public ICommand DeleteBomItemCommand { get; }
        public ICommand SelectBomItemsCommand { get; }
        public ICommand SelectNotFilledBomItemsCommand { get; }
        public ICommand ApplySearchOnSelectedBomItemCommand { get; }
        public ICommand FilterIsRequiredBomItemCommand { get; }
        public ICommand ExpandBomItemToAllLevelsCommand { get; }

        public ICommand SearchBomItemCommand { get; }
        public ICommand ClearSearchBomItemCommand { get; }

        public ICommand FilterBomItemCommand { get; }
        public ICommand ClearFilterBomItemCommand { get; }

        public ICommand ExpandBomItemCommand { get; }
        public ICommand CollapseBomItemCommand { get; }

        public ICommand OpenCreateRouteChartsFormCommand { get; }
        public CreateRouteChartCommand CreateOneRouteChartCommand { get; }
        public ICommand OpenMapDefectToDecisionEditFormCommand { get; }
        public ICommand FillBomItemsWithRequiredReplaceCommand { get; }
        public ICommand FillFinalDecisionBasedOnPmControlCommand { get; }

        public LoadBomItemsCommand LoadBomItemsCommand { get; }
        public LoadBomItemCommand LoadBomItemCommand { get; }

        public AsyncCommandBase SaveDefectPropsAndMoveNextCommand { get; }
        public AsyncCommandBase SaveDefectPropsOnAssemblyCommand { get; }
        public AsyncCommandBase SaveDefectPropsOnSelectedBomItemsCommand { get; }
        public AsyncCommandBase SaveDefectPropsOnAssemblyWithNotFilledRowsCommand { get; }
        
        #endregion

        #region Constructor

        private DefectListItemViewModel(
            IGetAllPlanOperationDtoQuery getAllPlanOperationDtoQuery,
            IGetAllAuxiliaryMaterialDtoQuery getAllAuxiliaryMaterialDtoQuery,
            IGetAllOgmetMatlDtoQuery getAllOgmetMatlDtoQuery,
            BomHeadersStore bomHeadersStore,
            SelectedBomItemStore selectedBomItemStore,
            BomItemsStore bomItemsStore,
            DefectToDecisionMapsStore defectToDecisionMapsStore,
            SelectedDefectToDecisionMapStore selectedDefectToDecisionMapStore,
            ProductsStore productsStore,
            TehprocHeadersStore tehprocHeadersStore,
            ISaveBomItemCommand saveBomItemCommand,
            IBomItemsValidator bomItemsValidator)
        {
            _bomItemViewModels = new ObservableCollection<BomItemViewModel>();
            _bomItemsView = CollectionViewSource.GetDefaultView(_bomItemViewModels);
            _bomHeadersStore = bomHeadersStore;
            _selectedBomItemStore = selectedBomItemStore;
            _bomItemsStore = bomItemsStore;
            _defectToDecisionMapsStore = defectToDecisionMapsStore;
            _tehprocHeadersStore = tehprocHeadersStore;
            _message = _defaultMessage;

            UserIdentity = (Thread.CurrentPrincipal.Identity as CustomIdentity);
            BomItemsFilterByDetalTypViewModel = new BomItemsFilterByDetalTypViewModel(this);
            Detals = new ObservableCollection<string>(new List<string>());
            PossibleDecisions = new ObservableCollection<string>(new List<string>());

            SetReadOnlyFieldsPermissions();

            IsVisibleAction = !PermissionsStore.IsPassiveUser;
            IsCanExecuteAction = PermissionsStore.IsWriteAccessUser || PermissionsStore.IsCanCreateRouteMapsUser;
            IsCanEditBomAction = PermissionsStore.IsSuperUser;
            SelectedFilteredColumn = FilteredColumns[0];

            ExportDefectListWithFinalDecisionToPdfCommand = new ExportDefectListToPdfCommand(this, _bomItemsStore, true);
            ExportDefectListWithInitialDecisionToPdfCommand = new ExportDefectListToPdfCommand(this, _bomItemsStore, false);
            ExportAssemblyWithFinalDecisionToPdfCommand = new ExportDefectListToPdfCommand(this, _bomItemsStore, true, FilterByStructureNumber());
            ExportAssemblyWithInitialDecisionToPdfCommand = new ExportDefectListToPdfCommand(this, _bomItemsStore, false, FilterByStructureNumber());
            ExportDefectListToExcelCommand = new ExportDefectListToExcelCommand(this, _bomItemsStore, getAllPlanOperationDtoQuery);
            ExportDefectListReclamationToPdfCommand = new ExportDefectListReclamationToPdfCommand(this, _bomItemsStore, model => model.IsSelected);
            ExportAssemblyReclamationToPdfCommand = new ExportDefectListReclamationToPdfCommand(this, _bomItemsStore, FilterByStructureNumber());
            PurchaseItemsReportCommand = new PurchaseItemsReportCommand(this, _bomItemsStore, bomHeadersStore);
            ScrapItemsReportCommand = new ScrapItemsReportCommand(this, _bomItemsStore);
            DefectListItemsChangesReportCommand = new DefectListItemsChangesReportCommand(this, _bomItemsStore);
            AdditionalMaterialsReportCommand = new AdditionalMaterialsReportCommand(this, _bomItemsStore, getAllAuxiliaryMaterialDtoQuery, getAllOgmetMatlDtoQuery);

            OpenAddBomItemFormCommand = new OpenAddBomItemFormCommand(this, _selectedBomItemStore, productsStore, _bomItemsStore);
            OpenReplaceBomItemFormCommand = new OpenReplaceBomItemFormCommand(this, _selectedBomItemStore, productsStore, _bomItemsStore);
            OpenReplaceNameBomItemFormCommand = new OpenReplaceNameBomItemFormCommand(this, _selectedBomItemStore, productsStore, _bomItemsStore);
            DeleteBomItemCommand = new DeleteBomItemCommand(this, _selectedBomItemStore, _bomItemsStore);
            SelectBomItemsCommand = new SelectBomItemsCommand(this, FilterByStructureNumber());
            SelectNotFilledBomItemsCommand = new SelectBomItemsCommand(this, FilterByStructureNumberWithNotFilledRows());
            FilterIsRequiredBomItemCommand = new FilterIsRequiredBomItemCommand(this);
            ExpandBomItemToAllLevelsCommand = new ExpandBomItemCommand(this, bomItemsStore,true);
            
            SearchBomItemCommand = new SearchBomItemCommand(this);
            ClearSearchBomItemCommand = new ClearSearchBomItemCommand(this);
            ApplySearchOnSelectedBomItemCommand = new ApplySearchOnSelectedBomItemCommand(this);
            FilterBomItemCommand = new FilterBomItemCommand(this);
            ClearFilterBomItemCommand = new ClearFilterBomItemCommand(this);
            ExpandBomItemCommand = new ExpandBomItemCommand(this, bomItemsStore, false);
            CollapseBomItemCommand = new CollapseBomItemCommand(this, bomItemsStore);

            OpenCreateRouteChartsFormCommand = new OpenCreateRouteChartsFormCommand(this);
            CreateOneRouteChartCommand = new CreateRouteChartCommand(this);
            OpenMapDefectToDecisionEditFormCommand = new OpenMapDefectToDecisionEditFormCommand(this, defectToDecisionMapsStore, selectedDefectToDecisionMapStore);
            FillBomItemsWithRequiredReplaceCommand = new FillBomItemsWithRequiredReplaceCommand(this, bomItemsStore, saveBomItemCommand, bomItemsValidator);
            FillFinalDecisionBasedOnPmControlCommand = new FillFinalDecisionBasedOnPmControlCommand(this, bomItemsStore, bomHeadersStore, saveBomItemCommand, bomItemsValidator);

            LoadBomItemsCommand = new LoadBomItemsCommand(this, _bomItemsStore, defectToDecisionMapsStore, tehprocHeadersStore);
            LoadBomItemCommand = new LoadBomItemCommand(this, _bomItemsStore, _selectedBomItemStore, tehprocHeadersStore);

            SaveDefectPropsAndMoveNextCommand = new SaveDefectPropsAndMoveNextCommand(this, bomItemsStore, saveBomItemCommand, bomItemsValidator);
            SaveDefectPropsOnAssemblyCommand = new SaveDefectPropsOnAssemblyCommand(this, bomItemsStore, saveBomItemCommand, bomItemsValidator);
            SaveDefectPropsOnSelectedBomItemsCommand = new SaveDefectPropsOnSelectedBomItemsCommand(this, bomItemsStore, saveBomItemCommand, bomItemsValidator);
            SaveDefectPropsOnAssemblyWithNotFilledRowsCommand = new SaveDefectPropsOnAssemblyWithNotFilledRowsCommand(this, bomItemsStore, saveBomItemCommand, bomItemsValidator);

            _defectToDecisionMapsStore.DefectToDecisionMapsLoaded += DefectToDecisionMapsStoreOnDefectToDecisionMapsLoaded;
            _bomItemsStore.BomItemsLoaded += BomItemsStoreOnBomItemsLoaded;
            _bomItemsStore.BomItemsLoadedById += BomItemsStoreOnBomItemsLoadedById;
            _bomItemsStore.BomItemsAdded += BomItemsStoreOnBomItemsEdited;
            _bomItemsStore.BomItemsReplaced += BomItemsStoreOnBomItemsEdited;
            _bomItemsStore.BomItemsNameReplaced += BomItemsStoreOnBomItemsEdited;
            _bomItemsStore.BomItemsDeleted += BomItemsStoreOnBomItemsEdited;
            _bomItemsView.CollectionChanged += BomItemsViewOnCollectionChanged;
            _selectedBomItemStore.SelectedBomItemChanged += SelectedBomItemStoreOnSelectedBomItemChanged;
        }

        public static DefectListItemViewModel LoadViewModel(
            IGetAllPlanOperationDtoQuery getAllPlanOperationDtoQuery,
            IGetAllAuxiliaryMaterialDtoQuery getAllAuxiliaryMaterialDtoQuery,
            IGetAllOgmetMatlDtoQuery getAllOgmetMatlDtoQuery,
            BomHeadersStore bomHeadersStore,
            SelectedBomItemStore selectedBomItemStore,
            BomItemsStore bomItemsStore,
            DefectToDecisionMapsStore defectToDecisionMapsStore,
            SelectedDefectToDecisionMapStore selectedDefectToDecisionMapStore,
            ProductsStore productsStore,
            TehprocHeadersStore tehprocHeadersStore,
            ISaveBomItemCommand saveBomItemCommand,
            IBomItemsValidator bomItemsValidator)
        {
            var viewModel = new DefectListItemViewModel(
                getAllPlanOperationDtoQuery,
                getAllAuxiliaryMaterialDtoQuery,
                getAllOgmetMatlDtoQuery,
                bomHeadersStore,
                selectedBomItemStore,
                bomItemsStore,
                defectToDecisionMapsStore,
                selectedDefectToDecisionMapStore,
                productsStore,
                tehprocHeadersStore,
                saveBomItemCommand,
                bomItemsValidator);
            viewModel.LoadBomItemsCommand?.Execute();
            return viewModel;
        }

        protected override void Dispose()
        {
            _bomItemsStore.BomItemsLoaded -= BomItemsStoreOnBomItemsLoaded;
            _bomItemsStore.BomItemsLoadedById -= BomItemsStoreOnBomItemsLoadedById;
            _bomItemsStore.BomItemsAdded -= BomItemsStoreOnBomItemsEdited;
            _bomItemsStore.BomItemsReplaced -= BomItemsStoreOnBomItemsEdited;
            _bomItemsStore.BomItemsNameReplaced -= BomItemsStoreOnBomItemsEdited;
            _bomItemsStore.BomItemsDeleted -= BomItemsStoreOnBomItemsEdited;
            _bomItemsView.CollectionChanged -= BomItemsViewOnCollectionChanged;

            base.Dispose();
        }

        #endregion

        #region EventSubscription
        private void BomItemsViewOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            decimal countFilledItems = _bomItemsView.OfType<BomItemViewModel>().Count(x => x.IsFilled);
            decimal countTotal = _bomItemsView.OfType<BomItemViewModel>().Count();

            FillPercentage = countTotal == 0 ? 0 : Math.Floor(countFilledItems / countTotal * (decimal)100.0);
            FillText = $"Заполнение: {FillPercentage}% ({countFilledItems} из {countTotal})";
        }

        private void BomItemsStoreOnBomItemsLoaded()
        {
            var selectedBomItemId = SelectedBomItemViewModel?.Id ?? 0;

            // После выполнения строки в SelectedDefectListItem присваивается null (поведение WPF по умолчанию)
            _bomItemViewModels.Clear();

            foreach (var bomItem in _bomItemsStore.BomItems.IsShowed())
            {
                var bomItemViewModel = new BomItemViewModel(bomItem, _defectToDecisionMapsStore, _tehprocHeadersStore, this);
                bomItemViewModel.PropertyChanged += BomItemModelOnPropertyChanged;
                bomItemViewModel.DecisionChanged += BomItemDecisionChanged;
                bomItemViewModel.FinalDecisionChanged += BomItemFinalDecisionChanged;

                _bomItemViewModels.Add(bomItemViewModel);
            }

            _bomItemsView = CollectionViewSource.GetDefaultView(_bomItemViewModels);
            _bomItemsView.Filter = OnBomItemsFiltered;

            var currentSearchString = SearchString;
            var currentFilterString = FilterString;

            Detals.Clear();
            var detals = _bomItemViewModels.Select(x => x.Detal).Distinct().OrderBy(x => x).ToList();
            detals.ForEach(x => Detals.Add(x));

            SearchString = currentSearchString;
            FilterString = currentFilterString;

            SelectedBomItemViewModel = _bomItemViewModels.FirstOrDefault(x =>
                x.Id == selectedBomItemId) ?? _bomItemViewModels.FirstOrDefault();
        }

        private void BomItemsStoreOnBomItemsLoadedById(BomItem bomItem)
        {
            var obj = _bomItemViewModels.FirstOrDefault(x => x.Id == bomItem.Id);
            if (obj != null)
            {
                obj.Update(bomItem);
                SelectedBomItemViewModel = obj;
            }
        }

        private void DefectToDecisionMapsStoreOnDefectToDecisionMapsLoaded()
        {
            var defectToDecisionMaps = _defectToDecisionMapsStore
                .DefectToDecisionMaps
                .ToList()
                .Select(x => new DefectToDecisionMapCheckBoxViewModel(x, MapDefectToDecisionChanged))
                .ToList();
            DefectToDecisionMaps = CollectionViewSource.GetDefaultView(new ObservableCollection<DefectToDecisionMapCheckBoxViewModel>(defectToDecisionMaps));
            DefectToDecisionMaps.GroupDescriptions?.Add(new PropertyGroupDescription(nameof(DefectToDecisionMapCheckBoxViewModel.Name)));

            PossibleDecisions.Clear();
            PossibleDecisions.Add(string.Empty);
            var possibleDecisions = _defectToDecisionMapsStore.Decision.Select(x => x).ToList();
            possibleDecisions.ForEach(x => PossibleDecisions.Add(x));
        }

        private void SelectedBomItemStoreOnSelectedBomItemChanged()
        {
            InfoMessage = _defaultMessage;
            SelectedPossibleDefect = null;
            DefectToDecisionMaps?.OfType<DefectToDecisionMapCheckBoxViewModel>()?.Where(x => x.IsSelected).ToList().ForEach(x => x.ResetSelected());
            CreateOneRouteChartCommand?.OnCanExecuteChanged();
            SaveDefectPropsAndMoveNextCommand?.OnCanExecuteChanged();
            SaveDefectPropsOnAssemblyCommand?.OnCanExecuteChanged();
            SaveDefectPropsOnAssemblyWithNotFilledRowsCommand?.OnCanExecuteChanged();
            SaveDefectPropsOnSelectedBomItemsCommand?.OnCanExecuteChanged();
            SelectedBomItemViewModel?.SetRepairDecisionCommand?.OnCanExecuteChanged();
            SelectedBomItemViewModel?.SetReplaceDecisionCommand?.OnCanExecuteChanged();
        }

        private bool OnBomItemsFiltered(object obj)
        {
            // Сначала применяется "Поиск", затем "Фильтрация" в рамках данных, отображаемых по результатам поиска

            // 0. Подготовка

            var bomItemViewModel = (BomItemViewModel)obj;
            var searchedBomItemIsRoot = _searchedBomItemParents.Count == 0;

            // 1. Поиск

            bool matchSearch = string.IsNullOrWhiteSpace(SearchString) || searchedBomItemIsRoot;
            if (!matchSearch)
            {
                matchSearch = _searchedBomItemParents.Any(x => bomItemViewModel.StructureNumber.StartsWith(x.StructureNumber));
            }

            // 2. Основная фильтрация

            bool matchFilter = string.IsNullOrWhiteSpace(FilterString);
            if (!matchFilter)
            {
                matchFilter = MatchFilter(bomItemViewModel, FilterString);
            }

            // 3. Дополнительная фильтрация

            var detalTypeFilterMode = BomItemsFilterByDetalTypViewModel.SelectedDetalType;
            bool matchAdditionalFilter;
            if (detalTypeFilterMode == "все типы")
                matchAdditionalFilter = true;
            else
            {
                var filterMode = new BomItemsFiltersStore.DetalTypFilter();
                if (detalTypeFilterMode == "сб.ед + покуп")
                {
                    matchAdditionalFilter = filterMode.Apply(bomItemViewModel, "сб.ед") && _bomItemViewModels.Any(x => x.ParentId == bomItemViewModel.Id && x.IsPki) || filterMode.Apply(bomItemViewModel, "покуп");
                }
                else
                    matchAdditionalFilter = bomItemViewModel.DetalTyp == detalTypeFilterMode;

                if (!matchFilter && BomItemsFilterByDetalTypViewModel.SelectedDetalType == "сб.ед + покуп")
                {
                    matchFilter = _bomItemViewModels.Any(x => x.ParentId == bomItemViewModel.Id && x.IsPki && MatchFilter(x, FilterString));
                }
            }

            // 4. Комбинирование результатов
            return matchSearch && matchFilter && matchAdditionalFilter;
        }

        private bool MatchFilter(BomItemViewModel bomItemViewModel, string filterString)
        {
            bool matchFilter;
            if (SelectedFilteredColumn == "По всем столбцам")
            {
                var res = new List<bool>();

                BomItemsConstantsStore.FilteredColumns.Skip(1).ToList()
                    .ForEach(x => res.Add(BomItemsConstantsStore.FilteredColumns[x.Key].Apply(bomItemViewModel, filterString)));

                matchFilter = res.Any(x => x);
            }
            else
                matchFilter = BomItemsConstantsStore.FilteredColumns[SelectedFilteredColumn]
                    .Apply(bomItemViewModel, filterString);

            return matchFilter;
        }

        private void BomItemsStoreOnBomItemsEdited()
        {
            LoadBomItemsCommand?.Execute();
            MessageBox.Show("Операция успешно выполнена");
        }

        #endregion

        #region BomItemViewModelEventSubscription
        public void BomItemDecisionChanged(BomItemViewModel obj, string decision)
        {
            obj.SetFinalDecisionCommand?.Execute(null);
            obj.SetTechnologicalProcessUsedCommand?.Execute(null);
            obj.SetRestoreReplaceQtyCommand?.Execute(null);
        }

        private void BomItemFinalDecisionChanged(BomItemViewModel obj, string finalDecision)
        {
            if (finalDecision == "ремонт")
                obj.SetTechnologicalProcessUsedCommand?.Execute(null);
            else
                obj.TechnologicalProcessUsed = null;
        }

        private void BomItemModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InfoMessage = _defaultMessage;
            SelectedBomItemViewModel?.SetRepairDecisionCommand?.OnCanExecuteChanged();
            SelectedBomItemViewModel?.SetReplaceDecisionCommand?.OnCanExecuteChanged();
            SaveDefectPropsAndMoveNextCommand?.OnCanExecuteChanged();
            SaveDefectPropsOnAssemblyCommand?.OnCanExecuteChanged();
            SaveDefectPropsOnAssemblyWithNotFilledRowsCommand?.OnCanExecuteChanged();
            SaveDefectPropsOnSelectedBomItemsCommand?.OnCanExecuteChanged();
        }

        public bool MapDefectToDecisionChanged(DefectToDecisionMapCheckBoxViewModel item, bool newValueIsSelected)
        {
            if (SelectedBomItemViewModel == null)
                return false;

            var isEmptyField = string.IsNullOrEmpty(SelectedBomItemViewModel.Defect);
            var items = DefectToDecisionMaps.OfType<DefectToDecisionMapCheckBoxViewModel>().Where(x => x.IsSelected).ToList();
            string defect = item.Item.Defect;
            bool result = newValueIsSelected;

            if (newValueIsSelected)
            {
                // Запрет на выделение элемента
                var uniqueIsAllowCombine = items.Select(x => x.Item.IsAllowCombine).Distinct().ToList();

                if (uniqueIsAllowCombine.Count == 1 &&
                    (uniqueIsAllowCombine.First() != item.Item.IsAllowCombine || (uniqueIsAllowCombine.First() == false && item.Item.IsAllowCombine == false)))
                    result = false;

                if (result)
                {
                    SelectedBomItemViewModel.Defect += isEmptyField ? defect : $", {defect}";

                    if (isEmptyField || !items.Any())
                    {
                        SelectedBomItemViewModel.Decision = item.Item.Decision;
                    }
                    else
                    {
                        SelectedBomItemViewModel.Decision =
                            items.Select(x => x.Item.Decision).Distinct().FirstOrDefault();
                    }
                }
            }
            else if (!isEmptyField)
            {
                if (SelectedBomItemViewModel.Defect.StartsWith(defect) && SelectedBomItemViewModel.Defect == defect)
                {
                    SelectedBomItemViewModel.Defect = null;
                    SelectedBomItemViewModel.Decision = null;
                    SelectedBomItemViewModel.QtyRestore = 0;
                    SelectedBomItemViewModel.QtyReplace = 0;
                }
                else
                {
                    if (SelectedBomItemViewModel.Defect.StartsWith(defect) && SelectedBomItemViewModel.Defect != defect)
                    {
                        SelectedBomItemViewModel.Defect = SelectedBomItemViewModel.Defect.Remove(0, defect.Length + 2);
                    }
                    else
                    {
                        int pos = SelectedBomItemViewModel.Defect.IndexOf(defect);
                        if (pos >= 0)
                            SelectedBomItemViewModel.Defect =
                                SelectedBomItemViewModel.Defect.Remove(pos - 2, defect.Length + 2);
                    }
                }
            }

            return result;
        }
        #endregion

        private bool CanUserEditBomItem()
        {
            return (BomHeader.StateInfo.IsWip || BomHeader.StateInfo.IsWaitApproved) && PermissionsStore.IsWriteAccessUser ||
                   (BomHeader.StateInfo.IsWip || BomHeader.StateInfo.IsWaitApproved) && PermissionsStore.IsSuperUser ||
                   (BomHeader.StateInfo.IsApproved) && PermissionsStore.IsSuperUser;
        }
        
        public Func<IBomItem, bool> FilterByStructureNumber()
        {
            return b => b.StructureNumber.StartsWith(SelectedBomItemViewModel.StructureNumber);
        }

        public Func<IBomItem, bool> FilterByStructureNumberWithNotFilledRows()
        {
            return b => b.StructureNumber.StartsWith(SelectedBomItemViewModel.StructureNumber) &&
                        string.IsNullOrEmpty(b.Defect?.Trim()) && string.IsNullOrEmpty(b.Decision?.Trim()) &&
                        b.QtyRestore == 0 && b.QtyReplace == 0;
        }

        public async Task RefreshDefectListHeader()
        {
            BomHeader = await _bomHeadersStore.Load(BomHeader.BomId);
            IsCanEditBomItem = CanUserEditBomItem();

            if (IsCanEditBomItem)
                SetFieldsPermissions();
            else
                SetReadOnlyFieldsPermissions();

            NotifyPropertyChanged(nameof(SelectedBomItemViewModel));
        }

        private void SetReadOnlyFieldsPermissions()
        {
            FieldsPermissions = PermissionsStore.FieldPermissionToRoles["ОТК Деф.вед. чтение"]?.FieldPermissions;
        }

        private void SetFieldsPermissions()
        {
            var fieldPermissionToRoles = new List<FieldPermissionToRole>();
            foreach (var role in UserIdentity.Roles)
            {
                FieldPermissionToRole result;
                if (PermissionsStore.FieldPermissionToRoles.TryGetValue(role.RoleName, out result))
                    fieldPermissionToRoles.Add(result);
            }

            FieldsPermissions = fieldPermissionToRoles.OrderBy(x => x.Rank).FirstOrDefault()?.FieldPermissions ??
                                BomItemFieldPermissionToRoleStore.ReadOnyFieldsPermissionSet;
        }

        public Message InfoMessage
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyPropertyChanged("InfoMessage");
            }
        }

        public class Message
        {
            public string Text { get; set; }
            public Brush Color { get; set; }
        }

        public class SuccessMessage : Message
        {
            public SuccessMessage()
            {
                Text = "Действие выполнено успешно";
                Color = new SolidColorBrush(Colors.LightGreen);
            }
        }

        public class ErrorMessage : Message
        {
            public ErrorMessage()
            {
                Color = new SolidColorBrush(Colors.LightCoral);
            }
        }

        public class DefaultMessage : Message
        {
            public DefaultMessage()
            {
                Text = "";
                Color = new SolidColorBrush(Colors.WhiteSmoke);
            }
        }
    }
}