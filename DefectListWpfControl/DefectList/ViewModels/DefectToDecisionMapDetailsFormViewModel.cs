using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class DefectToDecisionMapDetailsFormViewModel : ViewModel, IDataErrorInfo
    {
        private readonly DefectToDecisionMapsStore _defectToDecisionMapsStore;

        private string _defect;
        public string Defect
        {
            get { return _defect; }
            set
            {
                _defect = value;
                NotifyPropertyChanged(nameof(Defect));
                NotifyPropertyChanged(nameof(CanSubmit));
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
                NotifyPropertyChanged(nameof(CanSubmit));
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
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        private int _stateDetalsId;
        public int StateDetalsId
        {
            get { return _stateDetalsId; }
            set
            {
                _stateDetalsId = value;
                NotifyPropertyChanged(nameof(StateDetalsId));

                StateDetals = DetalStates.FirstOrDefault(x => x.Id == value);
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        private int _groupDefectId;
        public int GroupDefectId
        {
            get { return _groupDefectId; }
            set
            {
                _groupDefectId = value;
                NotifyPropertyChanged(nameof(GroupDefectId));

                GroupDefect = DefectGroups.FirstOrDefault(x => x.Id == value);
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        private StateDetals _stateDetals;
        public StateDetals StateDetals
        {
            get { return _stateDetals; }
            set
            {
                _stateDetals = value;
                NotifyPropertyChanged(nameof(StateDetals));
                NotifyPropertyChanged(nameof(CanSubmit));
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
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        private bool _isSubmitting;
        public bool IsSubmitting
        {
            get
            {
                return _isSubmitting;
            }
            set
            {
                _isSubmitting = value;
                NotifyPropertyChanged(nameof(IsSubmitting));
            }
        }

        public bool CanSubmit =>
            !string.IsNullOrEmpty(Defect) &&
            !string.IsNullOrEmpty(Decision) &&
            StateDetals != null &&
            GroupDefect != null &&
            IsAllValidProperties(this);

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

        public ObservableCollection<StateDetals> DetalStates => new ObservableCollection<StateDetals>(_defectToDecisionMapsStore.DetalStates);
        public ObservableCollection<GroupDefect> DefectGroups => new ObservableCollection<GroupDefect>(_defectToDecisionMapsStore.DefectGroups);
        public ObservableCollection<string> Decisions => new ObservableCollection<string>(new List<string>()
        {
            "заменить",
            "использовать",
            "ремонт",
            "ремонтное воздействие не требуется",
            "скомплектовать"
        });

        public ICommand SubmitCommand { get; }

        public DefectToDecisionMapDetailsFormViewModel(DefectToDecisionMapsStore defectToDecisionMapsStore, ICommand submitCommand)
        {
            _defectToDecisionMapsStore = defectToDecisionMapsStore;
            SubmitCommand = submitCommand;
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
                            error = $"Поле 'Дефект'. Превышена допустимая длина строки - {1000}";
                        break;
                    case nameof(Decision):
                        if (Decision?.Length > 300)
                            error = $"Поле 'Решение'. Превышена допустимая длина строки - {300}";
                        break;
                }

                return error;
            }
        }

        public bool IsAllValidProperties(DefectToDecisionMapDetailsFormViewModel vm)
        {
            var columns = new List<string>
            {
                vm[nameof(Defect)],
                vm[nameof(Decision)],
            };

            var errors = columns.Where(e => !string.IsNullOrEmpty(e)).ToList();
            return errors.Count == 0;
        }
    }
}