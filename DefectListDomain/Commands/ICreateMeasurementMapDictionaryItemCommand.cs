using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    // Создаёт новую строку справочника и пишет лог (Action=1).
    // Возвращает Id созданной строки.
    public interface ICreateMeasurementMapDictionaryItemCommand
    {
        Task<int> ExecuteAsync(MeasurementMapDictionaryItem item,
            IReadOnlyList<RecommendedRepairMethodAlternative> repairMethodAlternatives,
            string createdBy);
    }
}