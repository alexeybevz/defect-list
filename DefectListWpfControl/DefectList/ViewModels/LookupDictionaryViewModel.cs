using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Data;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Commands.MeasurementMapDictionaryCommands;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.ViewModels
{
    // Универсальная ViewModel для LookupDictionaryWindow.
    // Параметризуется через Kind — одна форма, одна ViewModel для всех 4 справочников.
    public class LookupDictionaryViewModel : ViewModel
    {
        private readonly LookupStore _store;
        private readonly ObservableCollection<LookupItemViewModel> _itemVms;

        public LookupItemKind Kind => _store.Kind;
        public string Title => GetTitle(_store.Kind);
        public CustomIdentity User { get; }
        public LoadingStateViewModel LoadingStateViewModel { get; } = new LoadingStateViewModel();

        public ICollectionView ItemsView { get; }

        private LookupItemViewModel _selectedItem;
        public LookupItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                // Отписываемся от предыдущей строки, чтоб не держать лишнюю ссылку
                if (_selectedItem != null)
                    _selectedItem.PropertyChanged -= OnSelectedItemPropertyChanged;
                _selectedItem = value;

                // Подписываемся на PropertyChanged новой строки
                // Когда пользователь редактирует Name в ячейке DataGrid,
                // LookupItemViewModel стреляет PropertyChanged("HasChanges") –
                // мы перехватываем это и инвалидируем CanExecute кнопки "Сохранить"
                if (_selectedItem != null)
                    _selectedItem.PropertyChanged += OnSelectedItemPropertyChanged;

                NotifyPropertyChanged(nameof(SelectedItem));
                SaveItemCommand.OnCanExecuteChanged();
            }
        }

        private void OnSelectedItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LookupItemViewModel.HasChanges))
                SaveItemCommand.OnCanExecuteChanged();
        }

        // Поле для создания нового элемента
        private string _newItemName;
        public string NewItemName
        {
            get { return _newItemName; }
            set
            {
                _newItemName = value;
                NotifyPropertyChanged(nameof(NewItemName));
                CreateItemCommand.OnCanExecuteChanged();
            }
        }

        public CommandBase LoadItemsCommand { get; }
        public CommandBase SaveItemCommand { get; }
        public CommandBase CreateItemCommand { get; }

        public LookupDictionaryViewModel(LookupStore store)
        {
            _store = store;
            User = Thread.CurrentPrincipal.Identity as CustomIdentity;

            _itemVms = new ObservableCollection<LookupItemViewModel>();
            ItemsView = CollectionViewSource.GetDefaultView(_itemVms);

            LoadItemsCommand = new LoadLookupItemsCommand(LoadingStateViewModel, _store, this);
            SaveItemCommand = new SaveLookupItemCommand(LoadingStateViewModel, _store, this);
            CreateItemCommand = new CreateLookupItemCommand(LoadingStateViewModel, _store, this);

            _store.ItemsLoaded += RefreshItems;

            LoadItemsCommand.Execute(null);
        }

        internal void RefreshItems()
        {
            _itemVms.Clear();
            foreach (var item in _store.Items)
                _itemVms.Add(new LookupItemViewModel(item));
            NotifyPropertyChanged(nameof(ItemsView));
        }

        protected override void ExecuteDispose()
        {
            _store.ItemsLoaded -= RefreshItems;
        }

        private static string GetTitle(LookupItemKind kind)
        {
            switch (kind)
            {
                case LookupItemKind.PossibleDefect: return "Справочник: Возможные дефекты";
                case LookupItemKind.NominalValue: return "Справочник: Номинальные значения";
                case LookupItemKind.RecommendedRepairMethod: return "Справочник: Рекомендуемые методы ремонта";
                case LookupItemKind.RequirementPostRepair: return "Справочник: Требования после ремонта";
                default: return "Справочник";
            }
        }
    }
}
