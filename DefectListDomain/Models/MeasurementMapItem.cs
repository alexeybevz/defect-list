using System;
using System.Collections.Generic;

namespace DefectListDomain.Models
{
    public class MeasurementMapItem
    {
        public int Id { get; set; }
        public int MeasurementMapId { get; set; }
        public int? MeasurementMapDictionaryItemId { get; set; }
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
        public IReadOnlyCollection<RecommendedRepairMethodAlternative> RepairMethodAlternatives { get; set; }

        public int? AlternateNominalValueId { get; set; }
        public string AlternateNominalValueName { get; set; }

        // Заполняет пользователь
        public string ActualValue { get; set; }
        public DateTime? ActualValueRecordDate { get; set; }
        public int? ActualRecommendedRepairMethodId { get; set; }
        public string ActualRecommendedRepairMethodName { get; set; }
        public string MarkOfWorkCompletion { get; set; }
        public string MarkOfWorkCompletionBy { get; set; }

        public MeasurementMapItemType ItemType { get; set; }

        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime RecordDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public int RowVersion { get; set; }
    }
}