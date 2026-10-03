using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DefectListDomain.Models;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.ViewModels
{
    // ViewModel формы редактирования одной строки справочника.
    // Используется как для создания новой строки, так и для редактирования существующей.
    // После успешного сохранения вызывается SavedCallback — форма закрывается.
    public class MeasurementMapDictionaryItemEditViewModel : ViewModel
    {
        // Все доступные значения для ComboBox-ов
        public ObservableCollection<LookupItem> PossibleDefects { get; }
        public ObservableCollection<LookupItem> NominalValues { get; }
        public ObservableCollection<LookupItem> RecommendedRepairMethods { get; }
        public ObservableCollection<LookupItem> RequirementPostRepairs { get; }

        // Все варианты ItemType для ComboBox
        public IReadOnlyList<ItemTypeOption> ItemTypeOptions { get; } = new List<ItemTypeOption>
        {
            new ItemTypeOption { Value = MeasurementMapItemType.FreeText, DisplayName = "Свободный текст" },
            new ItemTypeOption { Value = MeasurementMapItemType.YesNo,    DisplayName = "Да/Нет" },
            new ItemTypeOption { Value = MeasurementMapItemType.Numeric,  DisplayName = "Числовой" }
        };

        // Редактируемые поля строки
        private string _customNumeration;
        public string CustomNumeration
        {
            get { return _customNumeration; }
            set { _customNumeration = value; NotifyPropertyChanged(nameof(CustomNumeration)); }
        }

        private int _sortOrder;
        public int SortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; NotifyPropertyChanged(nameof(SortOrder)); }
        }

        private LookupItem _selectedPossibleDefect;
        public LookupItem SelectedPossibleDefect
        {
            get { return _selectedPossibleDefect; }
            set { _selectedPossibleDefect = value; NotifyPropertyChanged(nameof(SelectedPossibleDefect)); }
        }

        private LookupItem _selectedNominalValue;
        public LookupItem SelectedNominalValue
        {
            get { return _selectedNominalValue; }
            set { _selectedNominalValue = value; NotifyPropertyChanged(nameof(SelectedNominalValue)); }
        }

        private LookupItem _selectedAlternateNominalValue;
        public LookupItem SelectedAlternateNominalValue
        {
            get { return _selectedAlternateNominalValue; }
            set { _selectedAlternateNominalValue = value; NotifyPropertyChanged(nameof(SelectedAlternateNominalValue)); }
        }

        private LookupItem _selectedRecommendedRepairMethod;
        public LookupItem SelectedRecommendedRepairMethod
        {
            get { return _selectedRecommendedRepairMethod; }
            set { _selectedRecommendedRepairMethod = value; NotifyPropertyChanged(nameof(SelectedRecommendedRepairMethod)); }
        }

        private LookupItem _selectedRequirementPostRepair;
        public LookupItem SelectedRequirementPostRepair
        {
            get { return _selectedRequirementPostRepair; }
            set { _selectedRequirementPostRepair = value; NotifyPropertyChanged(nameof(SelectedRequirementPostRepair)); }
        }

        private MeasurementMapItemType _selectedItemType;
        public MeasurementMapItemType SelectedItemType
        {
            get { return _selectedItemType; }
            set { _selectedItemType = value; NotifyPropertyChanged(nameof(SelectedItemType)); }
        }

        // Выбранные альтернативы метода ремонта (управляются через отдельное окно)
        public ObservableCollection<RecommendedRepairMethodAlternative> SelectedAlternatives { get; }

        // Отображаем выбранные альтернативы одной строкой на кнопке
        public string AlternativesDisplay =>
            SelectedAlternatives.Any()
                ? string.Join(", ", SelectedAlternatives.Select(a => a.RecommendedRepairMethodName))
                : "Не выбраны";

        public bool IsEditMode { get; }
        public string Title => IsEditMode ? "Редактирование строки справочника" : "Новая строка справочника";
        public LoadingStateViewModel LoadingStateViewModel { get; } = new LoadingStateViewModel();
        public CustomIdentity User { get; }

        // Команды
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand OpenAlternativesCommand { get; }

        // Исходная строка (null = создание новой)
        private readonly MeasurementMapDictionaryItem _originalItem;
        private readonly int _dictionaryId;

        // Callback при успешном сохранении — закрывает окно
        public Action SavedCallback { get; set; }

        // Инжектируется снаружи — метод сохранения через Store
        // Принимает (item, alternativeIds, user) → Task
        private readonly Func<MeasurementMapDictionaryItem, IReadOnlyList<RecommendedRepairMethodAlternative>, string, Task> _saveAction;

        public MeasurementMapDictionaryItemEditViewModel(
            MeasurementMapDictionaryItem originalItem,  // null для создания
            int dictionaryId,
            int itemsLastSequence,
            IReadOnlyList<LookupItem> possibleDefects,
            IReadOnlyList<LookupItem> nominalValues,
            IReadOnlyList<LookupItem> repairMethods,
            IReadOnlyList<LookupItem> requirementsPostRepair,
            Func<MeasurementMapDictionaryItem, IReadOnlyList<RecommendedRepairMethodAlternative>, string, Task> saveAction)
        {
            _originalItem = originalItem;
            _dictionaryId = dictionaryId;
            _saveAction = saveAction;
            User = Thread.CurrentPrincipal.Identity as CustomIdentity;
            IsEditMode = originalItem != null;

            // Только активные элементы в ComboBox
            PossibleDefects = new ObservableCollection<LookupItem>(possibleDefects.Where(x => x.IsActive));
            NominalValues = new ObservableCollection<LookupItem>(nominalValues.Where(x => x.IsActive));
            RecommendedRepairMethods = new ObservableCollection<LookupItem>(repairMethods.Where(x => x.IsActive));
            RequirementPostRepairs = new ObservableCollection<LookupItem>(requirementsPostRepair.Where(x => x.IsActive));

            SelectedAlternatives = new ObservableCollection<RecommendedRepairMethodAlternative>();
            SelectedAlternatives.CollectionChanged += (s, e) => NotifyPropertyChanged(nameof(AlternativesDisplay));

            if (IsEditMode)
            {
                // Заполняем поля из существующей строки
                _customNumeration = originalItem.CustomNumeration;
                _sortOrder = originalItem.SortOrder;
                _selectedItemType = originalItem.ItemType;
                _selectedPossibleDefect = PossibleDefects.FirstOrDefault(x => x.Id == originalItem.PossibleDefectId);
                _selectedNominalValue = NominalValues.FirstOrDefault(x => x.Id == originalItem.NominalValueId);
                _selectedAlternateNominalValue = NominalValues.FirstOrDefault(x => x.Id == originalItem.AlternateNominalValueId);
                _selectedRecommendedRepairMethod = RecommendedRepairMethods.FirstOrDefault(x => x.Id == originalItem.RecommendedRepairMethodId);
                _selectedRequirementPostRepair = RequirementPostRepairs.FirstOrDefault(x => x.Id == originalItem.RequirementPostRepairId);

                if (originalItem.RepairMethodAlternatives != null)
                {
                    foreach (var alt in originalItem.RepairMethodAlternatives)
                        SelectedAlternatives.Add(alt);
                }
            }
            else
            {
                _selectedItemType = MeasurementMapItemType.YesNo;
                SortOrder = itemsLastSequence + 1;
            }

            SaveCommand = new DelegateCommand(async _ =>
            {
                await LoadingStateViewModel.ExecuteWithLoadingAsync(async () =>
                {
                    var item = BuildModel();
                    var altIds = SelectedAlternatives
                        .ToList();

                    await _saveAction(item, altIds, User?.Name);
                    SavedCallback?.Invoke();
                });
            }, _ => true);

            OpenAlternativesCommand = new DelegateCommand(_ =>
            {
                // Открывается окно выбора альтернатив.
                // Вызывается из code-behind формы (передаём текущий список методов и коллбэк).
                OpenAlternativesRequested?.Invoke();
            });
        }

        // Событие-запрос на открытие окна выбора альтернатив
        // (code-behind подпишется и откроет нужное окно)
        public event Action OpenAlternativesRequested;

        public MeasurementMapDictionaryItem BuildModel()
        {
            return new MeasurementMapDictionaryItem
            {
                Id = _originalItem?.Id ?? 0,
                MeasurementMapDictionaryId = _dictionaryId,
                CustomNumeration = _customNumeration,
                SortOrder = _sortOrder,
                PossibleDefectId = _selectedPossibleDefect?.Id ?? 0,
                PossibleDefectName = _selectedPossibleDefect?.Name,
                NominalValueId = _selectedNominalValue?.Id,
                NominalValueName = _selectedNominalValue?.Name,
                AlternateNominalValueId = _selectedAlternateNominalValue?.Id,
                AlternateNominalValueName = _selectedAlternateNominalValue?.Name,
                RecommendedRepairMethodId = _selectedRecommendedRepairMethod?.Id,
                RecommendedRepairMethodName = _selectedRecommendedRepairMethod?.Name,
                RequirementPostRepairId = _selectedRequirementPostRepair?.Id,
                RequirementPostRepairName = _selectedRequirementPostRepair?.Name,
                ItemType = _selectedItemType
            };
        }

        public class ItemTypeOption
        {
            public MeasurementMapItemType Value { get; set; }
            public string DisplayName { get; set; }
        }
    }
}