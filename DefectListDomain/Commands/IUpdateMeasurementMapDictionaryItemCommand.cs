using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    // Обновляет существующую строку справочника и пишет лог (Action=2).
    // Альтернативы ремонта заменяются целиком (DELETE + INSERT).
    public interface IUpdateMeasurementMapDictionaryItemCommand
    {
        Task ExecuteAsync(MeasurementMapDictionaryItem item,
            IReadOnlyList<RecommendedRepairMethodAlternative> repairMethodAlternatives,
            string updatedBy);
    }
}