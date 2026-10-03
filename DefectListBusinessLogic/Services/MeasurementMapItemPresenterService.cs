using DefectListDomain.Models;
using DefectListDomain.Services;

namespace DefectListBusinessLogic.Services
{
    public class MeasurementMapItemPresenterService : IMeasurementMapItemPresenterService
    {
        public MeasurementMapItemDisplayModel Present(MeasurementMapItem item)
        {
            // Правило: рекомендуемый метод ремонта показывается только если 
            // фактическое значение заполнено и отличается от номинального
            var hasDeviation = !string.IsNullOrEmpty(item.ActualValue) &&
                               item.ActualValue != item.NominalValueName;

            return new MeasurementMapItemDisplayModel()
            {
                PossibleDefectName = item.CustomNumeration + " " + item.PossibleDefectName,
                NominalValueName = item.NominalValueName,
                ActualValue = item.ActualValue,
                ActualValueRecordDate = item.ItemType == MeasurementMapItemType.YesNo && item.NominalValueName == item.ActualValue ? item.ActualValueRecordDate : null,
                ActualRecommendedRepairMethodName = hasDeviation ? item.ActualRecommendedRepairMethodName : string.Empty,
                MarkOfWorkCompletion = item.MarkOfWorkCompletion,
                RequirementPostRepairName = item.RequirementPostRepairName
            };
        }
    }
}