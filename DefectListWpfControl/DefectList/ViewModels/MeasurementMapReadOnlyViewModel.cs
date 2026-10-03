using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Data;
using DefectListDomain.Models;
using DefectListDomain.Services;
using DefectListWpfControl.DefectList.Commands.MeasurementMapCommands;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class MeasurementMapReadOnlyViewModel : ViewModel
    {
        private readonly MeasurementMapStore _measurementMapStore;
        private readonly IMeasurementMapItemPresenterService _measurementMapItemPresenterService;
        private readonly ObservableCollection<MeasurementMapItemDisplayModel> _items;

        public ICollectionView ItemsView { get; }
        public bool HasMap => _measurementMapStore.MeasurementMap != null;
        public int? Id => _measurementMapStore.MeasurementMap?.Id;
        public string FormName => _measurementMapStore.MeasurementMap?.MeasurementsMapTypeFormName;
        public IReadOnlyList<string> SketchFilePath => _measurementMapStore.MeasurementMap?.SketchFilePaths;
        public string Title => "Карта измерения № " + _measurementMapStore.MeasurementMap?.Id;
        public CustomIdentity User { get; }
        public LoadingStateViewModel LoadingStateViewModel { get; } = new LoadingStateViewModel();

        public AsyncCommandBase ExportMapCommand { get; }
        public AsyncCommandBase LoadMapCommand { get; }
        public AsyncCommandBase OpenReadOnlyFormMapCommand { get; }
        public AsyncCommandBase OpenFormSketchViewCommand { get; }

        public MeasurementMapReadOnlyViewModel(
            DefectListItemViewModel defectListItemViewModel,
            MeasurementMapStore measurementMapStore,
            SelectedBomItemStore selectedBomItemStore,
            IMeasurementMapItemPresenterService measurementMapItemPresenterService)
        {
            _measurementMapStore = measurementMapStore;
            _measurementMapItemPresenterService = measurementMapItemPresenterService;
            User = Thread.CurrentPrincipal.Identity as CustomIdentity;

            _items = new ObservableCollection<MeasurementMapItemDisplayModel>();
            ItemsView = CollectionViewSource.GetDefaultView(_items);

            LoadMapCommand = new LoadMeasurementMapCommand(LoadingStateViewModel, _measurementMapStore, selectedBomItemStore);
            ExportMapCommand = new ExportMeasurementMapCommand(defectListItemViewModel,  _measurementMapStore, selectedBomItemStore);
            OpenReadOnlyFormMapCommand = new OpenMeasurementMapReadOnlyFormCommand(this);
            OpenFormSketchViewCommand = new OpenFormSketchViewCommand(_measurementMapStore, selectedBomItemStore);

            _measurementMapStore.MeasurementMapLoaded += RefreshItems;
        }

        private void RefreshItems()
        {
            _items.Clear();

            if (_measurementMapStore.MeasurementMap?.Items != null)
            {
                var measurementMapItems = _measurementMapStore
                    .MeasurementMap
                    .Items
                    .OrderBy(i => i.SortOrder)
                    .ToList();

                foreach (var item in measurementMapItems)
                    _items.Add(_measurementMapItemPresenterService.Present(item));

                NotifyPropertyChanged(nameof(HasMap));
                NotifyPropertyChanged(nameof(FormName));
            }
        }

        protected override void ExecuteDispose()
        {
            _measurementMapStore.MeasurementMapLoaded -= RefreshItems;
        }
    }
}