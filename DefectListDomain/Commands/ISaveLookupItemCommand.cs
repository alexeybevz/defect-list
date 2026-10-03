using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    // Два сценария использования:
    //
    // 1. Запись НЕ используется → переименовать (UPDATE Name WHERE Id = @Id).
    // 2. Запись используется → деактивировать старую (IsActive=0) + INSERT новой.
    //    Возвращает Id новой записи (нужен чтобы обновить ItemSource в форме).
    //
    // Реализация сама определяет сценарий по IsUsed,
    // вызывающий код просто передаёт оба значения.
    public interface ISaveLookupItemCommand
    {
        Task<int> ExecuteAsync(LookupItemKind kind, int id, string newName, bool isUsed, string updatedBy);
    }
}