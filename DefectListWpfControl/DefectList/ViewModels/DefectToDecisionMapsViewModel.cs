using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Commands.DefectToDecisionMapCommands;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class DefectToDecisionMapsViewModel : ViewModel
    {
        private readonly DefectToDecisionMapsStore _defectToDecisionMapsStore;
        private readonly SelectedDefectToDecisionMapStore _selectedDefectToDecisionMapStore;

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

        public DefectToDecisionMapViewModel SelectedDefectToDecisionMapViewModel
        {
            get
            {
                return _defectToDecisionMapViewModels.FirstOrDefault(x =>
                    x.Id == _selectedDefectToDecisionMapStore.SelectedDefectToDecisionMap?.Id);
            }
            set
            {
                _selectedDefectToDecisionMapStore.SelectedDefectToDecisionMap = value?.MapDefectToDecision;
                NotifyPropertyChanged(nameof(SelectedDefectToDecisionMapViewModel));
            }
        }

        public CustomIdentity UserIdentity { get; }

        private readonly ObservableCollection<DefectToDecisionMapViewModel> _defectToDecisionMapViewModels;
        public ICollectionView DefectToDecisionMapsView { get; private set; }

        public ICommand OpenAddDefectToDecisionMapCommand { get; }
        public ICommand OpenEditDefectToDecisionMapCommand { get; }
        public ICommand DeleteDefectToDecisionMapCommand { get; }
        public ICommand LoadDefectToDecisionMapsCommand { get; }

        private DefectToDecisionMapsViewModel(DefectToDecisionMapsStore defectToDecisionMapsStore, SelectedDefectToDecisionMapStore selectedDefectToDecisionMapStore)
        {
            UserIdentity = Thread.CurrentPrincipal.Identity as CustomIdentity;
            _defectToDecisionMapsStore = defectToDecisionMapsStore;
            _selectedDefectToDecisionMapStore = selectedDefectToDecisionMapStore;
            _defectToDecisionMapViewModels = new ObservableCollection<DefectToDecisionMapViewModel>();

            _defectToDecisionMapViewModels = new ObservableCollection<DefectToDecisionMapViewModel>();
            DefectToDecisionMapsView = CollectionViewSource.GetDefaultView(_defectToDecisionMapViewModels);

            OpenAddDefectToDecisionMapCommand = new OpenAddDefectToDecisionMapCommand(_defectToDecisionMapsStore);
            OpenEditDefectToDecisionMapCommand = new OpenEditDefectToDecisionMapCommand(this, _defectToDecisionMapsStore);
            DeleteDefectToDecisionMapCommand = new DeleteDefectToDecisionMapCommand(_defectToDecisionMapsStore, _selectedDefectToDecisionMapStore);
            LoadDefectToDecisionMapsCommand = new LoadDefectToDecisionMapsCommand(this, defectToDecisionMapsStore);

            _defectToDecisionMapsStore.DefectToDecisionMapsLoaded += DefectToDecisionMapsStoreOnDefectToDecisionMapsLoaded;
            _defectToDecisionMapsStore.DefectToDecisionMapAdded += DefectToDecisionMapsStoreOnDefectToDecisionMapAdded;
            _defectToDecisionMapsStore.DefectToDecisionMapUpdated += DefectToDecisionMapsStoreOnDefectToDecisionMapUpdated;
            _defectToDecisionMapsStore.DefectToDecisionMapDeleted += DefectToDecisionMapsStoreOnDefectToDecisionMapDeleted;
            _selectedDefectToDecisionMapStore.SelectedBomHeaderChanged += SelectedDefectToDecisionMapStoreOnSelectedBomHeaderChanged;
        }

        protected override void Dispose()
        {
            _defectToDecisionMapsStore.DefectToDecisionMapsLoaded -= DefectToDecisionMapsStoreOnDefectToDecisionMapsLoaded;
            _defectToDecisionMapsStore.DefectToDecisionMapAdded -= DefectToDecisionMapsStoreOnDefectToDecisionMapAdded;
            _defectToDecisionMapsStore.DefectToDecisionMapUpdated -= DefectToDecisionMapsStoreOnDefectToDecisionMapUpdated;
            _defectToDecisionMapsStore.DefectToDecisionMapDeleted -= DefectToDecisionMapsStoreOnDefectToDecisionMapDeleted;
            _selectedDefectToDecisionMapStore.SelectedBomHeaderChanged -= SelectedDefectToDecisionMapStoreOnSelectedBomHeaderChanged;

            base.Dispose();
        }

        public static DefectToDecisionMapsViewModel LoadViewModel(DefectToDecisionMapsStore defectToDecisionMapsStore, SelectedDefectToDecisionMapStore selectedDefectToDecisionMapStore)
        {
            var vm = new DefectToDecisionMapsViewModel(defectToDecisionMapsStore, selectedDefectToDecisionMapStore);
            vm.LoadDefectToDecisionMapsCommand?.Execute(null);
            return vm;
        }

        private void DefectToDecisionMapsStoreOnDefectToDecisionMapsLoaded()
        {
            var selectedDefectToDecisionMapId = SelectedDefectToDecisionMapViewModel?.Id ?? 0;

            _defectToDecisionMapViewModels.Clear();

            foreach (var defectToDecisionMap in _defectToDecisionMapsStore.DefectToDecisionMaps.OrderBy(x => x.Defect).ToList())
            {
                _defectToDecisionMapViewModels.Add(new DefectToDecisionMapViewModel(defectToDecisionMap));
            }

            DefectToDecisionMapsView = CollectionViewSource.GetDefaultView(_defectToDecisionMapViewModels);

            SelectedDefectToDecisionMapViewModel = _defectToDecisionMapViewModels.FirstOrDefault(x =>
                x.Id == selectedDefectToDecisionMapId);
        }

        private void DefectToDecisionMapsStoreOnDefectToDecisionMapAdded(MapDefectToDecision mapDefectToDecision)
        {
            _defectToDecisionMapViewModels.Insert(0, new DefectToDecisionMapViewModel(mapDefectToDecision));
        }

        private void DefectToDecisionMapsStoreOnDefectToDecisionMapUpdated(MapDefectToDecision mapDefectToDecision)
        {
            var defectToDecisionMapViewModel = _defectToDecisionMapViewModels.FirstOrDefault(x => x.MapDefectToDecision?.Id == mapDefectToDecision.Id);
            defectToDecisionMapViewModel?.Update(mapDefectToDecision);
            NotifyPropertyChanged(nameof(SelectedDefectToDecisionMapViewModel));
        }

        private void DefectToDecisionMapsStoreOnDefectToDecisionMapDeleted(int id)
        {
            var defectToDecisionMapViewModel = _defectToDecisionMapViewModels.FirstOrDefault(x => x.MapDefectToDecision?.Id == id);
            if (defectToDecisionMapViewModel != null)
            {
                _defectToDecisionMapViewModels.Remove(defectToDecisionMapViewModel);
            }
        }

        private void SelectedDefectToDecisionMapStoreOnSelectedBomHeaderChanged()
        {
            NotifyPropertyChanged(nameof(SelectedDefectToDecisionMapViewModel));
        }
   }
}