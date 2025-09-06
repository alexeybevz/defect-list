using System;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using DefectListDomain.Models;
using DefectListWpfControl.ViewModelImplement;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.Commands.BomHeaderCommands;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class BomHeaderViewModel : ObservableObject, IBomHeader
    {
        private readonly BomHeaderSubscribersStore _bomHeaderSubscribersStore;
        private int _bomId;
        private string _orders;
        private string _serialNumber;
        private string _serialNumberAfterRepair;
        private RootItem _rootItem;
        private int _izdelQty;
        private int _stateDetalsId;
        private string _comment;
        private decimal _totalRowsCount;
        private decimal _filledRowsCount;
        private DateTime _dateOfSpecif;
        private DateTime _dateOfTehproc;
        private DateTime _dateOfMtrl;
        private DateTime? _dateOfPreparation;
        private string _createdBy;
        private string _createdByName;
        private DateTime _createDate;
        private string _updatedBy;
        private string _updatedByName;
        private DateTime _recordDate;
        private BomHeaderState _state;
        private BomHeaderStateInfo _stateInfo;
        private string _contract;
        private DateTime _contractDateOpen;
        private DateTime? _createDateFirstRouteMap;
        private string _headerType;
        private bool _isUserSubscribeOnBomHeader;

        public BomHeader BomHeader { get; private set; }

        public ICommand SubscribeUserOnBomHeaderCommand { get; private set; }
        public ICommand UnSubscribeUserOnBomHeaderCommand { get; private set; }
        public ICommand ApproveCommand { get; private set; }
        public ICommand SetClosedStateCommand { get; private set; }
        public ICommand SetWorkInProgressStateCommand { get; private set; }
        public ICommand DoubleClickBomHeaderCommand { get; private set; }

        public BomHeaderViewModel(BomHeadersStore bomHeadersStore, BomHeaderSubscribersStore bomHeaderSubscribersStore, OpenedBomHeadersStore openedBomHeadersStore, BomHeader bomHeader)
        {
            var user = Thread.CurrentPrincipal.Identity as CustomIdentity;
            BomHeader = bomHeader;
            Update(bomHeader);

            _bomHeaderSubscribersStore = bomHeaderSubscribersStore;
            _bomHeaderSubscribersStore.BomHeaderSubscribersLoaded += () =>
            {
                
                IsUserSubscribeOnBomHeader = _bomHeaderSubscribersStore.BomHeaderSubscribers.Any(x => x.BomId == BomId && x.UserId == user.UserId);
            };

            CreateCommands(bomHeadersStore, openedBomHeadersStore, user);
        }

        private void CreateCommands(BomHeadersStore bomHeadersStore, OpenedBomHeadersStore openedBomHeadersStore, CustomIdentity user)
        {
            SubscribeUserOnBomHeaderCommand = new SubscribeUserOnBomHeaderCommand(this, _bomHeaderSubscribersStore);
            UnSubscribeUserOnBomHeaderCommand = new UnSubscribeUserOnBomHeaderCommand(this, _bomHeaderSubscribersStore);
            
            ApproveCommand = new UpdateStateBomHeaderCommand(this, bomHeadersStore, user.Name, BomHeaderState.Approved);
            SetClosedStateCommand = new UpdateStateBomHeaderCommand(this, bomHeadersStore, user.Name, BomHeaderState.Closed);
            SetWorkInProgressStateCommand = new UpdateStateBomHeaderCommand(this, bomHeadersStore, user.Name, BomHeaderState.WorkInProgress);

            DoubleClickBomHeaderCommand = new DoubleClickBomHeaderCommand(openedBomHeadersStore, this);
        }

        public void Update(BomHeader bh)
        {
            if (bh == null)
                return;

            BomHeader = bh;
            BomId = bh.BomId;
            Orders = bh.Orders;
            SerialNumber = bh.SerialNumber;
            SerialNumberAfterRepair = bh.SerialNumberAfterRepair;
            RootItem = bh.RootItem;
            IzdelQty = bh.IzdelQty;
            StateDetalsId = bh.StateDetalsId;
            Comment = bh.Comment;
            TotalRowsCount = bh.TotalRowsCount;
            FilledRowsCount = bh.FilledRowsCount;
            DateOfSpecif = bh.DateOfSpecif;
            DateOfTehproc = bh.DateOfTehproc;
            DateOfMtrl = bh.DateOfMtrl;
            DateOfPreparation = bh.DateOfPreparation;
            CreatedBy = bh.CreatedBy;
            CreatedByName = bh.CreatedByName;
            CreateDate = bh.CreateDate;
            UpdatedBy = bh.UpdatedBy;
            UpdatedByName = bh.UpdatedByName;
            RecordDate = bh.RecordDate;
            State = bh.State;
            Contract = bh.Contract;
            ContractDateOpen = bh.ContractDateOpen;
            StateInfo = bh.StateInfo;
            CreateDateFirstRouteMap = bh.CreateDateFirstRouteMap;
            HeaderType = bh.HeaderType;
        }

        public int BomId
        {
            get { return _bomId; }
            set
            {
                _bomId = value;
                NotifyPropertyChanged(nameof(BomId));
            }
        }

        public string Orders
        {
            get { return _orders; }
            set
            {
                _orders = value;
                NotifyPropertyChanged(nameof(Orders));
            }
        }

        public string SerialNumber
        {
            get { return _serialNumber; }
            set
            {
                _serialNumber = value;
                NotifyPropertyChanged(nameof(SerialNumber));
            }
        }

        public string SerialNumberAfterRepair
        {
            get { return _serialNumberAfterRepair; }
            set
            {
                _serialNumberAfterRepair = value;
                NotifyPropertyChanged(nameof(SerialNumberAfterRepair));
            }
        }

        public RootItem RootItem
        {
            get { return _rootItem; }
            set
            {
                _rootItem = value;
                NotifyPropertyChanged(nameof(RootItem));
            }
        }

        public int IzdelQty
        {
            get { return _izdelQty; }
            set
            {
                _izdelQty = value;
                NotifyPropertyChanged(nameof(IzdelQty));
            }
        }

        public int StateDetalsId
        {
            get { return _stateDetalsId; }
            set
            {
                _stateDetalsId = value;
                NotifyPropertyChanged(nameof(StateDetalsId));
            }
        }

        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                NotifyPropertyChanged(nameof(Comment));
            }
        }

        public decimal TotalRowsCount
        {
            get { return _totalRowsCount; }
            set
            {
                _totalRowsCount = value;
                NotifyPropertyChanged(nameof(TotalRowsCount));
            }
        }

        public decimal FilledRowsCount
        {
            get { return _filledRowsCount; }
            set
            {
                _filledRowsCount = value;
                NotifyPropertyChanged(nameof(FilledRowsCount));
            }
        }

        public DateTime DateOfSpecif
        {
            get { return _dateOfSpecif; }
            set
            {
                _dateOfSpecif = value;
                NotifyPropertyChanged(nameof(DateOfSpecif));
            }
        }

        public DateTime DateOfTehproc
        {
            get { return _dateOfTehproc; }
            set
            {
                _dateOfTehproc = value;
                NotifyPropertyChanged(nameof(DateOfTehproc));
            }
        }

        public DateTime DateOfMtrl
        {
            get { return _dateOfMtrl; }
            set
            {
                _dateOfMtrl = value;
                NotifyPropertyChanged(nameof(DateOfMtrl));
            }
        }

        public DateTime? DateOfPreparation
        {
            get { return _dateOfPreparation; }
            set
            {
                _dateOfPreparation = value;
                NotifyPropertyChanged(nameof(DateOfPreparation));
            }
        }

        public string CreatedBy
        {
            get { return _createdBy; }
            set
            {
                _createdBy = value;
                NotifyPropertyChanged(nameof(CreatedBy));
            }
        }

        public string CreatedByName
        {
            get { return _createdByName; }
            set
            {
                _createdByName = value;
                NotifyPropertyChanged(nameof(CreatedByName));
            }
        }

        public DateTime CreateDate
        {
            get { return _createDate; }
            set
            {
                _createDate = value;
                NotifyPropertyChanged(nameof(CreateDate));
            }
        }

        public string UpdatedBy
        {
            get { return _updatedBy; }
            set
            {
                _updatedBy = value;
                NotifyPropertyChanged(nameof(UpdatedBy));
            }
        }

        public string UpdatedByName
        {
            get { return _updatedByName; }
            set
            {
                _updatedByName = value;
                NotifyPropertyChanged(nameof(UpdatedByName));
            }
        }

        public DateTime RecordDate
        {
            get { return _recordDate; }
            set
            {
                _recordDate = value;
                NotifyPropertyChanged(nameof(RecordDate));
            }
        }

        public BomHeaderState State
        {
            get { return _state; }
            set
            {
                _state = value;
                NotifyPropertyChanged(nameof(State));
            }
        }

        public string Contract
        {
            get { return _contract; }
            set
            {
                _contract = value;
                NotifyPropertyChanged(nameof(Contract));
            }
        }

        public DateTime ContractDateOpen
        {
            get { return _contractDateOpen; }
            set
            {
                _contractDateOpen = value;
                NotifyPropertyChanged(nameof(ContractDateOpen));
            }
        }

        public BomHeaderStateInfo StateInfo
        {
            get { return _stateInfo; }
            set
            {
                _stateInfo = value;
                NotifyPropertyChanged(nameof(StateInfo));
            }
        }

        public DateTime? CreateDateFirstRouteMap
        {
            get { return _createDateFirstRouteMap; }
            set
            {
                _createDateFirstRouteMap = value;
                NotifyPropertyChanged(nameof(CreateDateFirstRouteMap));
            }
        }

        public bool IsUserSubscribeOnBomHeader
        {
            get { return _isUserSubscribeOnBomHeader; }
            set
            {
                _isUserSubscribeOnBomHeader = value;
                NotifyPropertyChanged(nameof(IsUserSubscribeOnBomHeader));
            }
        }

        public string HeaderType
        {
            get { return _headerType; }
            set
            {
                _headerType = value;
                NotifyPropertyChanged(nameof(HeaderType));
            }
        }
    }
}
