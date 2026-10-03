using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Data;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Commands.MeasurementMapCommands;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class MeasurementMapViewModel : ViewModel
    {
        private readonly MeasurementMapStore _measurementMapStore;
        private readonly SelectedBomItemStore _selectedBomItemStore;

        private readonly ObservableCollection<MeasurementMapItemViewModel> _itemViewModels;

        // --- Публичные свойства ---

        public ICollectionView ItemsView { get; private set; }

        public CustomIdentity User { get; }
        public bool HasMap => _measurementMapStore.MeasurementMap != null;
        public bool IsCompleted => _measurementMapStore.MeasurementMap?.Status == 2;
        public bool IsEditable => HasMap && !IsCompleted;
        public int? Id => _measurementMapStore.MeasurementMap?.Id;
        public string FormName => _measurementMapStore.MeasurementMap?.MeasurementsMapTypeFormName;
        public string Title => "Заполнение карты измерения № " + _measurementMapStore.MeasurementMap?.Id;
        public byte Status => _measurementMapStore.MeasurementMap?.Status ?? 0;
        public IReadOnlyList<string> SketchFilePath => _measurementMapStore.MeasurementMap?.SketchFilePaths;
        public LoadingStateViewModel LoadingStateViewModel { get; } = new LoadingStateViewModel();

        private string _serialNumber;
        public string SerialNumber
        {
            get { return _serialNumber; }
            set
            {
                _serialNumber = value;
                NotifyPropertyChanged(nameof(SerialNumber));
            }
        }

        // --- Команды ---

        public AsyncCommandBase LoadMapCommand { get; }
        public AsyncCommandBase CreateMapCommand { get; }
        public AsyncCommandBase ExportMapCommand { get; }
        public AsyncCommandBase OpenEditFormMapCommand { get; }
        public AsyncCommandBase SaveItemsCommand { get; }
        public AsyncCommandBase OpenFormSketchViewCommand { get; }

        public MeasurementMapViewModel(
            DefectListItemViewModel defectListItemViewModel,
            MeasurementMapStore store,
            SelectedBomItemStore selectedBomItemStore)
        {
            _measurementMapStore = store;
            _selectedBomItemStore = selectedBomItemStore;
            _itemViewModels = new ObservableCollection<MeasurementMapItemViewModel>();
            User = Thread.CurrentPrincipal.Identity as CustomIdentity;
            LoadingStateViewModel.IsLoading = false;

            // Инициализируем пустой View сразу, чтобы биндинг не падал до загрузки
            ItemsView = CollectionViewSource.GetDefaultView(_itemViewModels);

            LoadMapCommand = new LoadMeasurementMapCommand(LoadingStateViewModel, _measurementMapStore, selectedBomItemStore);
            CreateMapCommand = new CreateMeasurementMapCommand(this, defectListItemViewModel, _measurementMapStore, selectedBomItemStore);
            ExportMapCommand = new ExportMeasurementMapCommand(defectListItemViewModel, _measurementMapStore, selectedBomItemStore);
            OpenEditFormMapCommand = new OpenMeasurementMapEditFormCommand(this, defectListItemViewModel);
            SaveItemsCommand = new SaveMeasurementMapItemsCommand(this, _measurementMapStore, defectListItemViewModel);
            OpenFormSketchViewCommand = new OpenFormSketchViewCommand(_measurementMapStore, selectedBomItemStore);

            _measurementMapStore.MeasurementMapLoaded += Store_MapLoaded;
            _measurementMapStore.MeasurementMapCreated += Store_MapCreated;
            _measurementMapStore.MeasurementMapItemsSaved += Store_ItemsSaved;
        }

        // --- Обработчики Store ---

        private void Store_MapLoaded()
        {
            RefreshItems();
            RefreshCommandStates();
        }

        private void Store_MapCreated(MeasurementMap map)
        {
            RefreshItems();
            RefreshCommandStates();
        }

        private void Store_ItemsSaved()
        {
            foreach (var item in _itemViewModels)
                item.AcceptChanges();

            RefreshCommandStates();
        }

        // --- Вспомогательные методы ---

        private void RefreshItems()
        {
            _itemViewModels.Clear();

            if (_measurementMapStore.MeasurementMap?.Items != null)
            {
                foreach (var item in _measurementMapStore.MeasurementMap.Items)
                    _itemViewModels.Add(new MeasurementMapItemViewModel(item, User));
            }

            // Пересоздаём View после заполнения коллекции
            ItemsView = CollectionViewSource.GetDefaultView(_itemViewModels);
            NotifyPropertyChanged(nameof(ItemsView));
            NotifyPropertyChanged(nameof(HasMap));
            NotifyPropertyChanged(nameof(IsCompleted));
            NotifyPropertyChanged(nameof(IsEditable));
            NotifyPropertyChanged(nameof(Id));
            NotifyPropertyChanged(nameof(FormName));
            NotifyPropertyChanged(nameof(Status));

            SerialNumber = _selectedBomItemStore.SelectedBomItem?.SerialNumber;
        }

        private void RefreshCommandStates()
        {
            // Инвалидируем CanExecute у всех команд
            CreateMapCommand?.OnCanExecuteChanged();
            ExportMapCommand?.OnCanExecuteChanged();
            OpenEditFormMapCommand?.OnCanExecuteChanged();
            SaveItemsCommand?.OnCanExecuteChanged();
        }

        public void Dispose()
        {
            _measurementMapStore.MeasurementMapLoaded -= Store_MapLoaded;
            _measurementMapStore.MeasurementMapCreated -= Store_MapCreated;
            _measurementMapStore.MeasurementMapItemsSaved -= Store_ItemsSaved;
        }
    }
}