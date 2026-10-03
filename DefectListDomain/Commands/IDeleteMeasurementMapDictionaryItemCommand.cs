using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    // Помечает строку как удалённую (физическое DELETE из таблицы) и пишет лог (Action=3).
    // Удаление разрешено только если строка не используется ни в одном MeasurementMapItem.
    public interface IDeleteMeasurementMapDictionaryItemCommand
    {
        Task ExecuteAsync(MeasurementMapDictionaryItem item, string updatedBy);
    }
}