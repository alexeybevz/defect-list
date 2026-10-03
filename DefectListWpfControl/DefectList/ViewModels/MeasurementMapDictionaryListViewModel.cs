using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using DefectListDomain.Dtos;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Commands.MeasurementMapDictionaryCommands;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class MeasurementMapDictionaryListViewModel : ViewModel
    {
        private readonly MeasurementMapDictionaryStore _store;
        private readonly LookupStore _possibleDefectStore;
        private readonly LookupStore _nominalValueStore;
        private readonly LookupStore _repairMethodStore;
        private readonly LookupStore _requirementStore;
        private readonly ProductsStore _productsStore;

        private readonly ObservableCollection<MeasurementMapDictionaryViewModel> _items;
        private IReadOnlyList<ProductDto> _products;

        public CustomIdentity User { get; }
        public LoadingStateViewModel LoadingStateViewModel { get; } = new LoadingStateViewModel();

        // --- Список справочников ---

        public ICollectionView ItemsView { get; }

        private MeasurementMapDictionaryViewModel _selectedDictionary;
        public MeasurementMapDictionaryViewModel SelectedDictionary
        {
            get { return _selectedDictionary; }
            set
            {
                _selectedDictionary = value;
                NotifyPropertyChanged(nameof(SelectedDictionary));
                ArchiveDictionaryCommand.OnCanExecuteChanged();
                CreateVersionCommand.OnCanExecuteChanged();
                OpenItemsCommand.OnCanExecuteChanged();
            }
        }

        // --- Фильтр ---

        private int? _filterCodeLsf82;
        public int? FilterCodeLsf82
        {
            get { return _filterCodeLsf82; }
            set
            {
                _filterCodeLsf82 = value;
                NotifyPropertyChanged(nameof(FilterCodeLsf82));
                ItemsView.Refresh();
            }
        }

        private string _filterDetal;
        public string FilterDetal
        {
            get { return _filterDetal; }
            set
            {
                _filterDetal = value;
                NotifyPropertyChanged(nameof(FilterDetal));
                ItemsView.Refresh();
            }
        }

        private bool _showArchived;
        public bool ShowArchived
        {
            get { return _showArchived; }
            set
            {
                _showArchived = value;
                NotifyPropertyChanged(nameof(ShowArchived));
                ItemsView.Refresh();
            }
        }

        // --- Поля для создания нового справочника ---

        private string _newDictionaryName;
        public string NewDictionaryName
        {
            get { return _newDictionaryName; }
            set
            {
                _newDictionaryName = value;
                NotifyPropertyChanged(nameof(NewDictionaryName));
                CreateDictionaryCommand.OnCanExecuteChanged();
            }
        }

        private int _newDictionaryCodeLsf82;
        public int NewDictionaryCodeLsf82
        {
            get { return _newDictionaryCodeLsf82; }
            set
            {
                _newDictionaryCodeLsf82 = value;
                NotifyPropertyChanged(nameof(NewDictionaryCodeLsf82));
                CreateDictionaryCommand.OnCanExecuteChanged();
            }
        }

        private string _newDictionaryDetal;
        public string NewDictionaryDetal
        {
            get { return _newDictionaryDetal; }
            set
            {
                _newDictionaryDetal = value;
                NotifyPropertyChanged(nameof(NewDictionaryDetal));
                CreateDictionaryCommand.OnCanExecuteChanged();
            }
        }

        private int _newDictionaryFormId;
        public int NewDictionaryFormId
        {
            get { return _newDictionaryFormId; }
            set
            {
                _newDictionaryFormId = value;
                NotifyPropertyChanged(nameof(NewDictionaryFormId));
                CreateDictionaryCommand.OnCanExecuteChanged();
            }
        }

        private string _newDictionaryComment;
        public string NewDictionaryComment
        {
            get { return _newDictionaryComment; }
            set { _newDictionaryComment = value; NotifyPropertyChanged(nameof(NewDictionaryComment)); }
        }

        private string _copyMeasurementMapDictionaryIdStr;

        public string CopyMeasurementMapDictionaryIdStr
        {
            get { return _copyMeasurementMapDictionaryIdStr; }
            set
            {
                _copyMeasurementMapDictionaryIdStr = value;
                NotifyPropertyChanged(nameof(CopyMeasurementMapDictionaryIdStr));
                CreateDictionaryCommand.OnCanExecuteChanged();
            }
        }

        public int? CopyMeasurementMapDictionaryId
        {
            get
            {
                int id;
                var isParse = int.TryParse(CopyMeasurementMapDictionaryIdStr, out id);

                return isParse ? (int?) id : null;
            }
        }

        // Список изделий с галочками — аналог альтернативных методов ремонта
        public ObservableCollection<RootItemCheckBoxViewModel> RootItems { get; }

        // Выбранные изделия — передаются в команду создания
        public IReadOnlyList<RootItemCheckBoxViewModel> SelectedRootItems =>
            RootItems
                .Where(r => r.IsChecked)
                .ToList();

        public ObservableCollection<MeasurementsMapTypeForm> TypeForms { get; }

        // --- Команды справочника ---

        public AsyncCommandBase LoadDictionariesCommand { get; }
        public AsyncCommandBase CreateDictionaryCommand { get; }
        public AsyncCommandBase ArchiveDictionaryCommand { get; }
        public AsyncCommandBase CreateVersionCommand { get; }
        public AsyncCommandBase OpenItemsCommand { get; }
        public DelegateCommand OpenChoiceProductWindowCommand { get; }
        public DelegateCommand OpenChoiceCopyProductWindowCommand { get; }

        // --- Команды открытия lookup-справочников ---

        public DelegateCommand OpenPossibleDefectsCommand { get; }
        public DelegateCommand OpenNominalValuesCommand { get; }
        public DelegateCommand OpenRepairMethodsCommand { get; }
        public DelegateCommand OpenRequirementsCommand { get; }

        // Событие → code-behind открывает MeasurementMapDictionaryItemsWindow
        public event Action<int> OpenItemsRequested;

        // Событие → code-behind открывает LookupDictionaryWindow для нужного Store
        public event Action<LookupStore> OpenLookupRequested;

        public MeasurementMapDictionaryListViewModel(
            MeasurementMapDictionaryStore store,
            LookupStore possibleDefectStore,
            LookupStore nominalValueStore,
            LookupStore repairMethodStore,
            LookupStore requirementStore,
            ProductsStore productsStore,
            IReadOnlyList<MeasurementsMapTypeForm> typeForms)
        {
            _store = store;
            _possibleDefectStore = possibleDefectStore;
            _nominalValueStore = nominalValueStore;
            _repairMethodStore = repairMethodStore;
            _requirementStore = requirementStore;
            _productsStore = productsStore;
            User = Thread.CurrentPrincipal.Identity as CustomIdentity;

            _items = new ObservableCollection<MeasurementMapDictionaryViewModel>();
            ItemsView = CollectionViewSource.GetDefaultView(_items);
            ItemsView.Filter = OnFilter;
            ItemsView.SortDescriptions.Add(new SortDescription(
                nameof(MeasurementMapDictionaryViewModel.Code_LSF82), ListSortDirection.Ascending));
            ItemsView.SortDescriptions.Add(new SortDescription(
                nameof(MeasurementMapDictionaryViewModel.Version), ListSortDirection.Descending));

            TypeForms = new ObservableCollection<MeasurementsMapTypeForm>(typeForms);
            _products = new List<ProductDto>();
            // Инициализируем список изделий из Store (уже загружен при открытии формы)
            RootItems = new ObservableCollection<RootItemCheckBoxViewModel>();

            // Команды справочника
            LoadDictionariesCommand = new LoadMeasurementMapDictionaryListCommand(LoadingStateViewModel, _store);
            CreateDictionaryCommand = new CreateMeasurementMapDictionaryCommand(LoadingStateViewModel, _store, this);
            ArchiveDictionaryCommand = new ArchiveMeasurementMapDictionaryCommand(LoadingStateViewModel, _store, this);
            CreateVersionCommand = new CreateMeasurementMapDictionaryVersionCommand(LoadingStateViewModel, _store, this);
            OpenItemsCommand = new OpenMeasurementMapDictionaryItemsCommand(this);
            OpenChoiceProductWindowCommand = new DelegateCommand(_ => OpenChoiceProductWindow());

            // Команды открытия lookup-окон — DelegateCommand, т.к. логика тривиальная:
            // просто пробросить нужный Store в событие
            OpenPossibleDefectsCommand = new DelegateCommand(_ => OpenLookupRequested?.Invoke(_possibleDefectStore));
            OpenNominalValuesCommand = new DelegateCommand(_ => OpenLookupRequested?.Invoke(_nominalValueStore));
            OpenRepairMethodsCommand = new DelegateCommand(_ => OpenLookupRequested?.Invoke(_repairMethodStore));
            OpenRequirementsCommand = new DelegateCommand(_ => OpenLookupRequested?.Invoke(_requirementStore));

            _store.DictionariesLoaded += OnDictionariesLoaded;
            _store.DictionaryCreated += _ => OnDictionariesLoaded();
            _store.DictionaryCopied += _ => OnDictionariesLoaded();
            _store.DictionaryArchived += _ => OnDictionariesLoaded();
            _store.DictionaryVersionCreated += _ => OnDictionariesLoaded();

            LoadDictionariesCommand.Execute(null);
        }

        internal void RequestOpenItems(int dictionaryId)
        {
            OpenItemsRequested?.Invoke(dictionaryId);
        }

        // Строит ViewModel для формы строк конкретного справочника.
        // Вызывается из code-behind при открытии MeasurementMapDictionaryItemsWindow.
        public MeasurementMapDictionaryItemsViewModel BuildItemsViewModel(int dictionaryId)
        {
            return new MeasurementMapDictionaryItemsViewModel(
                dictionaryId,
                _store,
                _possibleDefectStore,
                _nominalValueStore,
                _repairMethodStore,
                _requirementStore);
        }

        public void ClearNewDictionaryFields()
        {
            NewDictionaryName = null;
            NewDictionaryCodeLsf82 = 0;
            NewDictionaryDetal = null;
            NewDictionaryFormId = 0;
            NewDictionaryComment = null;
            CopyMeasurementMapDictionaryIdStr = null;
        }

        private async void OnDictionariesLoaded()
        {
            _items.Clear();
            foreach (var d in _store.Dictionaries)
                _items.Add(new MeasurementMapDictionaryViewModel(d));
            ItemsView.Refresh();

            RootItems.Clear();
            foreach (var rootItem in _store.RootItems.OrderBy(x => x.Izdel).ToList())
                RootItems.Add(new RootItemCheckBoxViewModel(rootItem.Id, rootItem.Izdel));

            // Асинхронная подгрузка обозначения ДСЕ по CodeLSF82
            await Task.Run(async () =>
            {
                if (!_products.Any())
                    _products = (await _productsStore.GetAllDesignSpecifications()).ToList();

                var dict = _products
                    .GroupBy(k => k.CodeLsf82)
                    .ToDictionary(k => k.Key, v => v.FirstOrDefault());

                foreach (var vm in _items)
                {
                    ProductDto p;
                    if (dict.TryGetValue(vm.Code_LSF82, out p))
                        vm.Detal = p.Name;
                }
            });
        }

        private bool OnFilter(object obj)
        {
            var d = obj as MeasurementMapDictionaryViewModel;
            if (d == null) return false;
            if (!ShowArchived && !d.IsActive) return false;
            if (FilterCodeLsf82.HasValue && d.Code_LSF82 != FilterCodeLsf82.Value) return false;
            if (!string.IsNullOrEmpty(FilterDetal) && !string.IsNullOrEmpty(d.Detal) && !d.Detal.ToLower().Contains(FilterDetal.ToLower())) return false;
            return true;
        }

        private void OpenChoiceProductWindow()
        {
            var choiceProductVm = ChoiceProductViewModel.LoadViewModel(_productsStore);
            var choiceProductWindow = new ChoiceProductWindow(choiceProductVm);

            choiceProductVm.ProductSelected += product =>
            {
                var selectedProduct = choiceProductVm.SelectedProduct;
                NewDictionaryCodeLsf82 = selectedProduct?.Product.CodeLsf82 ?? 0;
                NewDictionaryDetal = selectedProduct?.Product.Name ?? null;
                NewDictionaryName = $"Шаблон {NewDictionaryDetal}";
                choiceProductWindow.Close();
            };

            choiceProductWindow.ShowDialog();
        }

        protected override void ExecuteDispose()
        {
            _store.DictionariesLoaded -= OnDictionariesLoaded;
            _store.DictionaryCreated -= _ => OnDictionariesLoaded();
            _store.DictionaryCopied -= _ => OnDictionariesLoaded();
            _store.DictionaryArchived -= _ => OnDictionariesLoaded();
            _store.DictionaryVersionCreated -= _ => OnDictionariesLoaded();
        }

        public class RootItemCheckBoxViewModel : ObservableObject
        {
            public int Id { get; }
            public string Name { get; }

            private bool _isChecked;
            public bool IsChecked
            {
                get { return _isChecked; }
                set
                {
                    _isChecked = value;
                    NotifyPropertyChanged(nameof(IsChecked));
                }
            }

            public RootItemCheckBoxViewModel(int id, string name, bool isChecked = false)
            {
                Id = id;
                Name = name;
                _isChecked = isChecked;
            }
        }
    }
}
