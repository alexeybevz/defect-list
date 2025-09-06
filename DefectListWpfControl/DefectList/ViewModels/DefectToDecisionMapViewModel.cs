using DefectListDomain.Models;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class DefectToDecisionMapViewModel : ViewModel
    {
        private int _id;
        public int Id
        {
            get {return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged(nameof(Id));
            }
        }

        private string _defect;
        public string Defect
        {
            get { return _defect; }
            set
            {
                _defect = value;
                NotifyPropertyChanged(nameof(Defect));
            }
        }

        private string _decision;
        public string Decision
        {
            get { return _decision; }
            set
            {
                _decision = value;
                NotifyPropertyChanged(nameof(Decision));
            }
        }

        private bool _isAllowCombine;
        public bool IsAllowCombine
        {
            get { return _isAllowCombine; }
            set
            {
                _isAllowCombine = value;
                NotifyPropertyChanged(nameof(IsAllowCombine));
            }
        }

        private StateDetals _stateDetal;
        public StateDetals StateDetals
        {
            get { return _stateDetal; }
            set
            {
                _stateDetal = value;
                NotifyPropertyChanged(nameof(StateDetals));
            }
        }

        private GroupDefect _groupDefect;
        public GroupDefect GroupDefect
        {
            get { return _groupDefect; }
            set
            {
                _groupDefect = value;
                NotifyPropertyChanged(nameof(GroupDefect));
            }
        }

        private MapDefectToDecision _mapDefectToDecision;
        public MapDefectToDecision MapDefectToDecision
        {
            get { return _mapDefectToDecision; }
            set
            {
                _mapDefectToDecision = value;
                NotifyPropertyChanged(nameof(MapDefectToDecision));
            }
        }

        public DefectToDecisionMapViewModel(MapDefectToDecision mapDefectToDecision)
        {
            Update(mapDefectToDecision);
        }

        public void Update(MapDefectToDecision mapDefectToDecision)
        {
            MapDefectToDecision = mapDefectToDecision;
            Id = mapDefectToDecision.Id;
            Defect = mapDefectToDecision.Defect;
            Decision = mapDefectToDecision.Decision;
            IsAllowCombine = mapDefectToDecision.IsAllowCombine;
            StateDetals = mapDefectToDecision.StateDetals;
            GroupDefect = mapDefectToDecision.GroupDefect;
        }
    }
}