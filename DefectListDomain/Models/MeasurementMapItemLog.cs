using System;

namespace DefectListDomain.Models
{
    public class MeasurementMapItemLog
    {
        public int Id { get; set; }
        public int MeasurementMapId { get; set; }
        public int MeasurementMapItemId { get; set; }
        public int Action { get; set; }
        public string ActionName
        {
            get {
                switch (Action)
                {
                    case 1: return "Вставка";
                    case 2: return "Изменение";
                    case 3: return "Удаление";
                    default:
                        return string.Empty;
                }
            }
        }
        public string CustomNumeration { get; set; }
        public int SortOrder { get; set; }
        public int PossibleDefectId { get; set; }
        public string PossibleDefectName { get; set; }
        public int? NominalValueId { get; set; }
        public string NominalValueName { get; set; }
        public int? RecommendedRepairMethodId { get; set; }
        public string RecommendedRepairMethodName { get; set; }
        public int? RequirementPostRepairId { get; set; }
        public string RequirementPostRepairName { get; set; }

        public int? AlternateNominalValueId { get; set; }
        public string AlternateNominalValueName { get; set; }

        // Заполняет пользователь
        public string ActualValue { get; set; }
        public int? ActualRecommendedRepairMethodId { get; set; }
        public string ActualRecommendedRepairMethodName { get; set; }
        public string MarkOfWorkCompletion { get; set; }
        public string MarkOfWorkCompletionBy { get; set; }
        public string MarkOfWorkCompletionByName { get; set; }

        public MeasurementMapItemType ItemType { get; set; }

        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
    }
}