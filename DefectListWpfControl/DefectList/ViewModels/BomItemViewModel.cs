using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Commands.BomItemCommands;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class BomItemViewModel : ObservableObject, IBomItemModel, IDataErrorInfo
    {
        private readonly TehprocHeadersStore _tehprocHeadersStore;

        private int _bomId;
        private int _id;
        private int? _parentId;
        private string _parentDetal;
        private string _parentDetalIma;
        private string _detals;
        private string _detal;
        private string _detalIma;
        private string _detalTyp;
        private string _detalUm;
        private float _qtyMnf;
        private float _qtyConstr;
        private float _qtyRestore;
        private float _qtyReplace;
        private string _comment;
        private string _defect;
        private string _decision;
        private string _commentDef;
        private string _serialNumber;
        private string _technologicalProcessUsed;
        private string _finalDecision;
        private IsBomItemRequiredSubmit _isRequiredSubmit;
        private IsBomItemSubmitted _isSubmitted;
        private DateTime _createDate;
        private string _createdBy;
        private string _createdByName;
        private DateTime _recordDate;
        private string _updatedBy;
        private string _updatedByName;
        private string _repairMethodName;
        private string _structureNumber;
        private int _orderByPriority;
        private byte _uzelFlag;
        private bool _isPki;
        private bool _isMaterial;
        private bool _isOtlivka;
        private bool _isScrap;
        private bool _isExistsRepairTechnologicalProcess;
        private bool _isExistsReplaceTechnologicalProcess;
        private bool _isSelected;
        private List<MapBomItemToRouteChart> _mapBomItemToRouteCharts;
        private bool? _isExpanded;
        private bool _isShowItem;
        private string _classifierID;
        private int? _productID;
        private int? _code_LSF82;
        private bool _isExpensive;
        private string _researchAction;
        private string _researchResult;

        public event Action<BomItemViewModel, string> DecisionChanged;
        public event Action<BomItemViewModel, string> FinalDecisionChanged;

        public DefectListItemViewModel DefectListItemViewModel { get; }

        public SetRepairDecisionCommand SetRepairDecisionCommand { get; }
        public SetReplaceDecisionCommand SetReplaceDecisionCommand { get; }
        public ICommand SetFinalDecisionCommand { get; }
        public ICommand SetRestoreReplaceQtyCommand { get; }
        public ICommand SetTechnologicalProcessUsedCommand { get; }

        public BomItemViewModel(IBomItem b)
        {
            Update(b);
        }

        public BomItemViewModel(IBomItem b, DefectToDecisionMapsStore defectToDecisionMapsStore,
            TehprocHeadersStore tehprocHeadersStore, DefectListItemViewModel defectListItemViewModel)
        {
            DefectListItemViewModel = defectListItemViewModel;
            _tehprocHeadersStore = tehprocHeadersStore;
            SetRepairDecisionCommand = new SetRepairDecisionCommand(this);
            SetReplaceDecisionCommand = new SetReplaceDecisionCommand(this);
            SetFinalDecisionCommand = new SetFinalDecisionCommand(this);
            SetTechnologicalProcessUsedCommand = new SetTechnologicalProcessUsedCommand(this);
            SetRestoreReplaceQtyCommand = new SetRestoreReplaceQtyCommand(this, defectToDecisionMapsStore);

            Update(b);
        }

        public void Update(IBomItem b)
        {
            BomItem = b as BomItem;
            IsSelected = false;
            BomId = b.BomId;
            Id = b.Id;
            ParentId = b.ParentId;
            ParentDetal = b.ParentDetal;
            ParentDetalIma = b.ParentDetalIma;
            Detals = b.Detals;
            Detal = b.Detal;
            DetalIma = b.DetalIma;
            DetalTyp = b.DetalTyp;
            DetalUm = b.DetalUm;
            QtyMnf = b.QtyMnf;
            QtyConstr = b.QtyConstr;
            QtyRestore = b.QtyRestore;
            QtyReplace = b.QtyReplace;
            Comment = b.Comment;
            Defect = b.Defect;
            Decision = b.Decision;
            CommentDef = b.CommentDef;
            SerialNumber = b.SerialNumber;
            FinalDecision = b.FinalDecision;
            TechnologicalProcessUsed = b.TechnologicalProcessUsed;
            IsRequiredSubmit = b.IsRequiredSubmit;
            IsSubmitted = b.IsSubmitted;
            CreateDate = b.CreateDate;
            CreatedBy = b.CreatedBy;
            CreatedByName = b.CreatedByName;
            RecordDate = b.RecordDate;
            UpdatedBy = b.UpdatedBy;
            UpdatedByName = b.UpdatedByName;
            RepairMethodName = b.RepairMethodName;
            StructureNumber = b.StructureNumber;
            _orderByPriority = b.OrderByPriority;
            UzelFlag = b.UzelFlag;
            _isPki = b.IsPki;
            _isMaterial = b.IsMaterial;
            _isOtlivka = b.IsOtlivka;
            _isScrap = b.IsScrap;
            MapBomItemToRouteCharts = b.MapBomItemToRouteCharts;
            IsExpanded = b.IsExpanded;
            IsShowItem = b.IsShowItem;
            ClassifierID = b.ClassifierID;
            ProductID = b.ProductID;
            Code_LSF82 = b.Code_LSF82;
            IsExpensive = b.IsExpensive;
            ResearchAction = b.ResearchAction;
            ResearchResult = b.ResearchResult;

            UpdateTehprocHeader();

            NotifyPropertyChanged(nameof(IsFilled));
            NotifyPropertyChanged(nameof(IsOnlyRestore));
            NotifyPropertyChanged(nameof(IsOnlyReplace));
            NotifyPropertyChanged(nameof(IsRestoreAndReplace));
            NotifyPropertyChanged(nameof(IsAcceptive));
            NotifyPropertyChanged(nameof(IsExistsRepairTechnologicalProcess));
            NotifyPropertyChanged(nameof(IsExistsReplaceTechnologicalProcess));
        }

        public void UpdateTehprocHeader()
        {
            TehprocHeader = _tehprocHeadersStore?.GetSummaryInfo(this);
            _isExistsRepairTechnologicalProcess = TehprocHeader?.IsRepair ?? false;
            _isExistsReplaceTechnologicalProcess = TehprocHeader?.IsReplace ?? false;
        }

        public BomItem BomItem { get; private set; }
        public TechnologicalProcessResult TehprocHeader { get; private set; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged(nameof(IsSelected));
            }
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

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged(nameof(Id));
            }
        }

        public int? ParentId
        {
            get { return _parentId; }
            set
            {
                _parentId = value;
                NotifyPropertyChanged(nameof(ParentId));
            }
        }

        public string ParentDetal
        {
            get { return _parentDetal; }
            set
            {
                _parentDetal = value;
                NotifyPropertyChanged(nameof(ParentDetal));
            }
        }

        public string ParentDetalIma
        {
            get { return _parentDetalIma; }
            set
            {
                _parentDetalIma = value;
                NotifyPropertyChanged(nameof(ParentDetalIma));
            }
        }

        public string Detals
        {
            get { return _detals; }
            set
            {
                _detals = value;
                NotifyPropertyChanged(nameof(Detals));
            }
        }

        public string Detal
        {
            get { return _detal; }
            set
            {
                _detal = value;
                NotifyPropertyChanged(nameof(Detal));
            }
        }

        public string DetalIma
        {
            get { return _detalIma; }
            set
            {
                _detalIma = value;
                NotifyPropertyChanged(nameof(DetalIma));
            }
        }

        public string DetalTyp
        {
            get { return _detalTyp; }
            set
            {
                _detalTyp = value;
                NotifyPropertyChanged(nameof(DetalTyp));
            }
        }

        public string DetalUm
        {
            get { return _detalUm; }
            set
            {
                _detalUm = value;
                NotifyPropertyChanged(nameof(DetalUm));
            }
        }

        public float QtyMnf
        {
            get { return _qtyMnf; }
            set
            {
                _qtyMnf = value;
                NotifyPropertyChanged(nameof(QtyMnf));
            }
        }

        public float QtyConstr
        {
            get { return _qtyConstr; }
            set
            {
                _qtyConstr = value;
                NotifyPropertyChanged(nameof(QtyConstr));
            }
        }

        public float QtyRestore
        {
            get { return _qtyRestore; }
            set
            {
                _qtyRestore = value;
                NotifyPropertyChanged(nameof(QtyRestore));
            }
        }

        public float QtyReplace
        {
            get { return _qtyReplace; }
            set
            {
                _qtyReplace = value;
                NotifyPropertyChanged(nameof(QtyReplace));
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

        public string Defect
        {
            get { return _defect; }
            set
            {
                _defect = value;
                NotifyPropertyChanged(nameof(Defect));
                NotifyPropertyChanged(nameof(IsValid));
            }
        }

        public string Decision
        {
            get { return _decision; }
            set
            {
                _decision = value;

                DecisionChanged?.Invoke(this, value);

                NotifyPropertyChanged(nameof(Decision));
                NotifyPropertyChanged(nameof(IsValid));
            }
        }

        public string CommentDef
        {
            get { return _commentDef; }
            set
            {
                _commentDef = value;
                NotifyPropertyChanged(nameof(CommentDef));
                NotifyPropertyChanged(nameof(IsValid));
            }
        }

        public string SerialNumber
        {
            get { return _serialNumber; }
            set
            {
                _serialNumber = value;
                NotifyPropertyChanged(nameof(SerialNumber));
                NotifyPropertyChanged(nameof(IsValid));
            }
        }

        public string TechnologicalProcessUsed
        {
            get { return _technologicalProcessUsed; }
            set
            {
                _technologicalProcessUsed = value;
                NotifyPropertyChanged(nameof(TechnologicalProcessUsed));
                NotifyPropertyChanged(nameof(IsValid));
            }
        }

        public string FinalDecision
        {
            get { return _finalDecision; }
            set
            {
                _finalDecision = value;

                FinalDecisionChanged?.Invoke(this, value);

                NotifyPropertyChanged(nameof(FinalDecision));
                NotifyPropertyChanged(nameof(IsValid));
            }
        }

        public IsBomItemRequiredSubmit IsRequiredSubmit
        {
            get { return _isRequiredSubmit; }
            set
            {
                _isRequiredSubmit = value;
                NotifyPropertyChanged(nameof(IsRequiredSubmit));
                NotifyPropertyChanged(nameof(IsRequiredSubmitText));
            }
        }

        public string IsRequiredSubmitText
        {
            get
            {
                switch (_isRequiredSubmit)
                {
                    case IsBomItemRequiredSubmit.Yes: return "Требуется предъявить ВП";
                    case IsBomItemRequiredSubmit.No: return "Не требует предъявления ВП";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public IsBomItemSubmitted IsSubmitted
        {
            get { return _isSubmitted; }
            set
            {
                _isSubmitted = value;
                NotifyPropertyChanged(nameof(IsSubmitted));
                NotifyPropertyChanged(nameof(IsSubmittedText));
            }
        }

        public string IsSubmittedText
        {
            get
            {
                switch (_isSubmitted)
                {
                    case IsBomItemSubmitted.Yes: return "Предъявлено ВП";
                    case IsBomItemSubmitted.No: return "Не предъявлено ВП";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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

        public DateTime RecordDate
        {
            get { return _recordDate; }
            set
            {
                _recordDate = value;
                NotifyPropertyChanged(nameof(RecordDate));
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

        public string RepairMethodName
        {
            get { return _repairMethodName; }
            set
            {
                _repairMethodName = value;
                NotifyPropertyChanged(nameof(RepairMethodName));
            }
        }

        public string StructureNumber
        {
            get { return _structureNumber; }
            set
            {
                _structureNumber = value;
                NotifyPropertyChanged(nameof(StructureNumber));
            }
        }

        public byte UzelFlag
        {
            get { return _uzelFlag; }
            set
            {
                _uzelFlag = value;
                NotifyPropertyChanged(nameof(UzelFlag));
            }
        }

        public int OrderByPriority => _orderByPriority;
        public bool IsPki => _isPki;
        public bool IsMaterial => _isMaterial;
        public bool IsOtlivka => _isOtlivka;
        public bool IsScrap => _isScrap;

        public bool IsFilled => !string.IsNullOrEmpty(Defect) && !string.IsNullOrEmpty(Decision);
        public bool IsOnlyRestore => IsFilled && QtyRestore > 0 && QtyReplace == 0;
        public bool IsOnlyReplace => IsFilled && QtyRestore == 0 && QtyReplace > 0;
        public bool IsRestoreAndReplace => IsFilled && QtyRestore > 0 && QtyReplace > 0;
        public bool IsAcceptive => QtyRestore == 0 && QtyReplace == 0 && IsFilled && (Decision == "использовать" || Decision == "ремонтное воздействие не требуется");

        public bool IsExistsRepairTechnologicalProcess => _isExistsRepairTechnologicalProcess;

        public bool IsExistsReplaceTechnologicalProcess => _isExistsReplaceTechnologicalProcess;

        public List<MapBomItemToRouteChart> MapBomItemToRouteCharts
        {
            get { return _mapBomItemToRouteCharts; }
            set
            {
                _mapBomItemToRouteCharts = value;
                NotifyPropertyChanged(nameof(MapBomItemToRouteCharts));
            }
        }

        public bool? IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                NotifyPropertyChanged(nameof(IsExpanded));
            }
        }

        public bool IsShowItem
        {
            get { return _isShowItem; }
            set
            {
                _isShowItem = value;
                NotifyPropertyChanged(nameof(IsShowItem));
            }
        }

        public string ClassifierID
        {
            get { return _classifierID; }
            set
            {
                _classifierID = value;
                NotifyPropertyChanged(nameof(ClassifierID));
            }
        }

        public int? ProductID
        {
            get { return _productID; }
            set
            {
                _productID = value;
                NotifyPropertyChanged(nameof(ProductID));
            }
        }

        public int? Code_LSF82
        {
            get { return _code_LSF82; }
            set
            {
                _code_LSF82 = value;
                NotifyPropertyChanged(nameof(Code_LSF82));
            }
        }

        public bool IsExpensive
        {
            get { return _isExpensive; }
            set
            {
                _isExpensive = value;
                NotifyPropertyChanged(nameof(IsExpensive));
            }
        }

        public string ResearchAction
        {
            get { return _researchAction; }
            set
            {
                _researchAction = value;
                NotifyPropertyChanged(nameof(ResearchAction));
            }
        }

        public string ResearchResult
        {
            get { return _researchResult; }
            set
            {
                _researchResult = value;
                NotifyPropertyChanged(nameof(ResearchResult));
            }
        }

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(Defect):
                        if (Defect?.Length > 1000)
                            error = $"Превышена допустимая длина строки - {1000}";
                        break;
                    case nameof(Decision):
                        if (Decision?.Length > 300)
                            error = $"Превышена допустимая длина строки - {300}";
                        break;
                    case nameof(CommentDef):
                        if (CommentDef?.Length > 200)
                            error = $"Превышена допустимая длина строки - {200}";
                        break;
                    case nameof(SerialNumber):
                        if (SerialNumber?.Length > 100)
                            error = $"Превышена допустимая длина строки - {100}";
                        break;
                    case nameof(TechnologicalProcessUsed):
                        if (TechnologicalProcessUsed?.Length > 200)
                            error = $"Превышена допустимая длина строки - {200}";
                        break;
                    case nameof(FinalDecision):
                        if (FinalDecision?.Length > 300)
                            error = $"Превышена допустимая длина строки - {300}";
                        break;
                    case nameof(ResearchAction):
                        if (ResearchAction?.Length > 300)
                            error = $"Превышена допустимая длина строки - {300}";
                        break;
                    case nameof(ResearchResult):
                        if (ResearchResult?.Length > 300)
                            error = $"Превышена допустимая длина строки - {300}";
                        break;
                }

                return error;
            }
        }

        public string Error { get; }

        public bool IsValid
        {
            get
            {
                var errors = new List<string>
                {
                    this[nameof(Defect)],
                    this[nameof(Decision)],
                    this[nameof(CommentDef)],
                    this[nameof(SerialNumber)],
                    this[nameof(TechnologicalProcessUsed)],
                    this[nameof(FinalDecision)],
                };
                return errors.Count(e => e != string.Empty) == 0;
            }
        }
    }
}