using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DefectListDomain.Models;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class RepairMethodAlternativesViewModel : ObservableObject
    {
        private readonly ObservableCollection<RecommendedRepairMethodAlternative> _target;

        public ObservableCollection<RepairMethodCheckboxViewModel> Options { get; }

        public string Title => "Альтернативы метода ремонта";

        public DelegateCommand ApplyCommand { get; }

        public System.Action AppliedCallback { get; set; }

        public RepairMethodAlternativesViewModel(
            IEnumerable<LookupItem> allMethods,
            ObservableCollection<RecommendedRepairMethodAlternative> currentAlternatives)
        {
            _target = currentAlternatives;

            var selectedIds = new HashSet<int>(currentAlternatives
                .Select(a => a.RecommendedRepairMethodId).ToList());

            Options = new ObservableCollection<RepairMethodCheckboxViewModel>(
                allMethods
                    .Where(m => m.IsActive)
                    .Select(m => new RepairMethodCheckboxViewModel(m, selectedIds.Contains(m.Id))));

            ApplyCommand = new DelegateCommand(_ =>
            {
                _target.Clear();
                foreach (var opt in Options.Where(o => o.IsSelected))
                {
                    _target.Add(new RecommendedRepairMethodAlternative
                    {
                        RecommendedRepairMethodId = opt.RecommendedRepairMethodId,
                        RecommendedRepairMethodName = opt.RecommendedRepairMethodName
                    });
                }
                AppliedCallback?.Invoke();
            });
        }
    }

    public class RepairMethodCheckboxViewModel : ObservableObject
    {
        public int RecommendedRepairMethodId { get; }
        public string RecommendedRepairMethodName { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; NotifyPropertyChanged(nameof(IsSelected)); }
        }

        public RepairMethodCheckboxViewModel(LookupItem method, bool isSelected)
        {
            RecommendedRepairMethodId = method.Id;
            RecommendedRepairMethodName = method.Name;
            _isSelected = isSelected;
        }
    }
}