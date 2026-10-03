using System.Collections.Generic;
using System.Linq;
using DefectListDomain.Models;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.ViewModels
{
    // ViewModel одной строки справочника карты измерений.
    // Используется в DataGrid формы MeasurementMapDictionaryItemsWindow.
    public class MeasurementMapDictionaryItemViewModel : ViewModel
    {
        private readonly MeasurementMapDictionaryItem _model;

        public int Id => _model.Id;
        public int MeasurementMapDictionaryId => _model.MeasurementMapDictionaryId;

        public string CustomNumeration => _model.CustomNumeration;
        public int SortOrder => _model.SortOrder;
        public string PossibleDefectName => _model.PossibleDefectName;
        public string NominalValueName => _model.NominalValueName;
        public string AlternateNominalValueName => _model.AlternateNominalValueName;
        public string RecommendedRepairMethodName => _model.RecommendedRepairMethodName;
        public string RequirementPostRepairName => _model.RequirementPostRepairName;
        public MeasurementMapItemType ItemType => _model.ItemType;

        // Отображаемое описание типа строки
        public string ItemTypeName
        {
            get
            {
                switch (_model.ItemType)
                {
                    case MeasurementMapItemType.FreeText: return "Свободный текст";
                    case MeasurementMapItemType.YesNo: return "Да/Нет";
                    case MeasurementMapItemType.Numeric: return "Числовой";
                    default: return string.Empty;
                }
            }
        }

        // Отображение альтернатив методов ремонта одной строкой для DataGrid
        public string RepairMethodAlternativesDisplay =>
            _model.RepairMethodAlternatives == null || !_model.RepairMethodAlternatives.Any()
                ? string.Empty
                : string.Join(", ", _model.RepairMethodAlternatives.Select(a => a.RecommendedRepairMethodName));

        // Список альтернатив для передачи в форму редактирования
        public IReadOnlyCollection<RecommendedRepairMethodAlternative> RepairMethodAlternatives =>
            _model.RepairMethodAlternatives;

        // Доменная модель — нужна для открытия формы редактирования
        public MeasurementMapDictionaryItem Model => _model;

        public MeasurementMapDictionaryItemViewModel(MeasurementMapDictionaryItem model)
        {
            _model = model;
        }
    }
}
