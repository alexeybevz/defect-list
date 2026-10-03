using System.Threading.Tasks;

namespace DefectListDomain.Commands
{
    // Создаёт новую версию справочника на основе существующего:
    // 1. Архивирует текущий активный справочник для данного Code_LSF82 (IsActive=0).
    // 2. Копирует шапку и все строки (с альтернативами) в новый справочник.
    // 3. Устанавливает Version = старый Version + 1, IsActive = 1.
    // 4. Пишет лог на оба справочника.
    // Возвращает Id нового справочника.
    public interface ICreateMeasurementMapDictionaryVersionCommand
    {
        Task<int> ExecuteAsync(int sourceDictionaryId, string createdBy);
    }
}