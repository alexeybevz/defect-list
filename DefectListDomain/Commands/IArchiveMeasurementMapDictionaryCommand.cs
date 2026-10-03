using System.Threading.Tasks;

namespace DefectListDomain.Commands
{
    // Архивирует справочник: устанавливает IsActive=0 и пишет лог.
    // Не создаёт новую версию — это делает ICreateMeasurementMapDictionaryVersionCommand.
    public interface IArchiveMeasurementMapDictionaryCommand
    {
        Task ExecuteAsync(int dictionaryId, string updatedBy);
    }
}