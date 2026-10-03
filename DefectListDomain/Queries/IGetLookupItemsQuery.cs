using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Queries
{
    // Загружает все записи выбранного справочника (включая неактивные —
    // чтобы администратор видел полную историю и мог понять, почему
    // нельзя переименовать используемую запись).
    // IsUsed проставляется внутри реализации одним дополнительным IN-запросом.
    public interface IGetLookupItemsQuery
    {
        Task<IReadOnlyList<LookupItem>> ExecuteAsync(LookupItemKind kind);
    }
}