using System;
using DefectListDomain.Models;

namespace DefectListWpfControl.DefectList.Stores
{
    public class SelectedBomHeaderStore
    {
        private readonly BomHeadersStore _bomHeadersStore;
        private BomHeader _selectedBomHeader;

        public BomHeader SelectedBomHeader
        {
            get { return _selectedBomHeader; }
            set
            {
                _selectedBomHeader = value;
                SelectedBomHeaderChanged?.Invoke();
            }
        }

        public event Action SelectedBomHeaderChanged;

        public SelectedBomHeaderStore(BomHeadersStore bomHeadersStore)
        {
            _bomHeadersStore = bomHeadersStore;

            _bomHeadersStore.BomHeaderAdded += BomHeadersStore_BomHeaderAdded;
            _bomHeadersStore.BomHeaderUpdated += BomHeadersStore_BomHeaderUpdated;
            _bomHeadersStore.BomHeaderDeleted += BomHeadersStore_BomHeaderDeleted;
            _bomHeadersStore.BomHeaderStateUpdated += BomHeadersStore_BomHeaderUpdated;
        }

        private void BomHeadersStore_BomHeaderAdded(BomHeader bomHeader)
        {
            SelectedBomHeader = bomHeader;
        }

        private void BomHeadersStore_BomHeaderUpdated(BomHeader bomHeader)
        {
            if (SelectedBomHeader?.BomId == bomHeader.BomId)
            {
                SelectedBomHeader = bomHeader;
            }
        }

        private void BomHeadersStore_BomHeaderDeleted(int id)
        {
            if (SelectedBomHeader?.BomId == id)
            {
                SelectedBomHeader = null;
            }
        }
    }
}