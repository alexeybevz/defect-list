using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Commands.MeasurementMapDictionaryCommands;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class MeasurementMapDictionaryItemsViewModel : ViewModel
    {
        private readonly MeasurementMapDictionaryStore _store;
        private readonly LookupStore _possibleDefectStore;
        private readonly LookupStore _nominalValueStore;
        private readonly LookupStore _repairMethodStore;
        private readonly LookupStore _requirementStore;

        private readonly ObservableCollection<MeasurementMapDictionaryItemViewModel> _itemVms;

        public CustomIdentity User { get; }
        public LoadingStateViewModel LoadingStateViewModel { get; } = new LoadingStateViewModel();
        public bool IsDictionaryActive => _store.CurrentDictionary?.IsActive ?? false;

        public string Title =>
            _store.CurrentDictionary != null
                ? $"Справочник КИ — Code_LSF82: {_store.CurrentDictionary.Code_LSF82}, " +
                  $"Версия: {_store.CurrentDictionary.Version}, " +
                  $"{_store.CurrentDictionary.MeasurementsMapTypeFormName}"
                : "Справочник карты измерений";

        public ICollectionView ItemsView { get; }

        private MeasurementMapDictionaryItemViewModel _selectedItem;
        public MeasurementMapDictionaryItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                NotifyPropertyChanged(nameof(SelectedItem));
                EditItemCommand.OnCanExecuteChanged();
                DeleteItemCommand.OnCanExecuteChanged();
            }
        }

        // --- Команды ---
        public AsyncCommandBase LoadItemsCommand { get; }
        public AsyncCommandBase CreateItemCommand { get; }
        public AsyncCommandBase EditItemCommand { get; }
        public AsyncCommandBase DeleteItemCommand { get; }

        // Событие → code-behind открывает MeasurementMapDictionaryItemEditWindow.
        // null = создание новой строки; не null = редактирование.
        public event Action<MeasurementMapDictionaryItem> OpenItemEditRequested;

        public MeasurementMapDictionaryItemsViewModel(
            int dictionaryId,
            MeasurementMapDictionaryStore store,
            LookupStore possibleDefectStore,
            LookupStore nominalValueStore,
            LookupStore repairMethodStore,
            LookupStore requirementStore)
        {
            _store = store;
            _possibleDefectStore = possibleDefectStore;
            _nominalValueStore = nominalValueStore;
            _repairMethodStore = repairMethodStore;
            _requirementStore = requirementStore;
            User = Thread.CurrentPrincipal.Identity as CustomIdentity;

            _itemVms = new ObservableCollection<MeasurementMapDictionaryItemViewModel>();
            ItemsView = CollectionViewSource.GetDefaultView(_itemVms);
            ItemsView.SortDescriptions.Add(new SortDescription(nameof(MeasurementMapDictionaryItemViewModel.SortOrder), ListSortDirection.Ascending));

            LoadItemsCommand = new LoadMeasurementMapDictionaryItemsCommand(LoadingStateViewModel, _store, dictionaryId);
            CreateItemCommand = new OpenCreateDictionaryItemCommand(this);
            EditItemCommand = new OpenEditDictionaryItemCommand(this);
            DeleteItemCommand = new DeleteDictionaryItemCommand(LoadingStateViewModel, _store, this);

            _store.CurrentDictionaryLoaded += OnCurrentDictionaryLoaded;
            _store.DictionaryItemCreated += _ => OnCurrentDictionaryLoaded();
            _store.DictionaryItemUpdated += _ => OnCurrentDictionaryLoaded();
            _store.DictionaryItemDeleted += _ => OnCurrentDictionaryLoaded();

            // Загружаем справочники для форм редактирования и сам словарь
            LoadItemsCommand.ExecuteAsync();
            _possibleDefectStore.LoadAsync();
            _nominalValueStore.LoadAsync();
            _repairMethodStore.LoadAsync();
            _requirementStore.LoadAsync();
        }

        // Вызывается из команд — инициирует открытие формы редактирования
        internal void RequestOpenItemEdit(MeasurementMapDictionaryItem item)
        {
            OpenItemEditRequested?.Invoke(item);
        }

        // Фабричный метод для построения ViewModel формы редактирования строки.
        // Вызывается из code-behind при открытии окна редактирования.
        public MeasurementMapDictionaryItemEditViewModel BuildItemEditViewModel(
            MeasurementMapDictionaryItem originalItem)
        {
            var dictionaryId = _store.CurrentDictionary?.Id ?? 0;
            var itemsLastSequence = _store.CurrentDictionary?.Items?.LastOrDefault()?.SortOrder ?? 0;

            return new MeasurementMapDictionaryItemEditViewModel(
                originalItem,
                dictionaryId,
                itemsLastSequence,
                _possibleDefectStore.Items,
                _nominalValueStore.Items,
                _repairMethodStore.Items,
                _requirementStore.Items,
                originalItem == null
                    ? (Func<MeasurementMapDictionaryItem, IReadOnlyList<RecommendedRepairMethodAlternative>, string, Task>)(
                        (item, altMethods, user) => _store.CreateItemAsync(item, altMethods, user))
                    : (item, altMethods, user) => _store.UpdateItemAsync(item, altMethods, user));
        }

        private void OnCurrentDictionaryLoaded()
        {
            _itemVms.Clear();

            if (_store.CurrentDictionary?.Items != null)
            {
                foreach (var item in _store.CurrentDictionary.Items.OrderBy(i => i.SortOrder))
                    _itemVms.Add(new MeasurementMapDictionaryItemViewModel(item));
            }

            NotifyPropertyChanged(nameof(Title));
            NotifyPropertyChanged(nameof(IsDictionaryActive));
            CreateItemCommand?.OnCanExecuteChanged();
            EditItemCommand?.OnCanExecuteChanged();
            DeleteItemCommand?.OnCanExecuteChanged();
        }

        protected override void ExecuteDispose()
        {
            _store.CurrentDictionaryLoaded -= OnCurrentDictionaryLoaded;
            _store.DictionaryItemCreated -= _ => OnCurrentDictionaryLoaded();
            _store.DictionaryItemUpdated -= _ => OnCurrentDictionaryLoaded();
            _store.DictionaryItemDeleted -= _ => OnCurrentDictionaryLoaded();
        }
    }
}