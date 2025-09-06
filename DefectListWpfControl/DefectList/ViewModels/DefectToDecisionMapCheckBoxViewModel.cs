using DefectListDomain.Models;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class DefectToDecisionMapCheckBoxViewModel : ObservableObject
    {
        private readonly MapDefectToDecisionChangedCmd _defectToDecisionMapChangedCommand;
        private bool _isSelected;
        private bool _isReset = false;

        public DefectToDecisionMapCheckBoxViewModel(MapDefectToDecision item, MapDefectToDecisionChangedCmd defectToDecisionMapChangedCommand)
        {
            Item = item;
            _defectToDecisionMapChangedCommand = defectToDecisionMapChangedCommand;
        }

        public MapDefectToDecision Item { get; }
        public string Name => Item.GroupDefect.GroupDefectName;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = _isReset ? value : _defectToDecisionMapChangedCommand.Invoke(this, value);
                NotifyPropertyChanged("IsSelected");
            }
        }

        public void ResetSelected()
        {
            _isReset = true;
            IsSelected = false;
            _isReset = false;
        }

        public override string ToString()
        {
            return Item.Defect;
        }
    }

    public delegate bool MapDefectToDecisionChangedCmd(DefectToDecisionMapCheckBoxViewModel item, bool newValueIsSelected);
}