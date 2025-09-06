using System;
using DefectListDomain.Models;

namespace DefectListWpfControl.DefectList.Stores
{
    public class SelectedDefectToDecisionMapStore
    {
        private readonly DefectToDecisionMapsStore _defectToDecisionMapsStore;
        private MapDefectToDecision _selectedBomHeader;

        public MapDefectToDecision SelectedDefectToDecisionMap
        {
            get { return _selectedBomHeader; }
            set
            {
                _selectedBomHeader = value;
                SelectedBomHeaderChanged?.Invoke();
            }
        }

        public event Action SelectedBomHeaderChanged;

        public SelectedDefectToDecisionMapStore(DefectToDecisionMapsStore defectToDecisionMapsStore)
        {
            _defectToDecisionMapsStore = defectToDecisionMapsStore;

            _defectToDecisionMapsStore.DefectToDecisionMapAdded += DefectToDecisionMapsStoreOnDefectToDecisionMapAdded;
            _defectToDecisionMapsStore.DefectToDecisionMapUpdated += DefectToDecisionMapsStoreOnDefectToDecisionMapUpdated;
            _defectToDecisionMapsStore.DefectToDecisionMapDeleted += DefectToDecisionMapsStoreOnDefectToDecisionMapDeleted ;
        }

        private void DefectToDecisionMapsStoreOnDefectToDecisionMapAdded(MapDefectToDecision mapDefectToDecision)
        {
            SelectedDefectToDecisionMap = mapDefectToDecision;
        }

        private void DefectToDecisionMapsStoreOnDefectToDecisionMapUpdated(MapDefectToDecision mapDefectToDecision)
        {
            if (SelectedDefectToDecisionMap?.Id == mapDefectToDecision.Id)
            {
                SelectedDefectToDecisionMap = mapDefectToDecision;
            }
        }

        private void DefectToDecisionMapsStoreOnDefectToDecisionMapDeleted(int id)
        {
            if (SelectedDefectToDecisionMap?.Id == id)
            {
                SelectedDefectToDecisionMap = null;
            }
        }
    }
}