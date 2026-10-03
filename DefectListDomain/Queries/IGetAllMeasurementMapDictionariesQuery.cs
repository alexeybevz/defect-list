using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Queries
{
    // Загружает список всех справочников карт измерений (без строк — только шапки).
    // Используется в списковой форме MeasurementMapDictionaryListWindow.
    // Включает неактивные (IsActive=0) — чтобы видеть историю версий.
    public interface IGetAllMeasurementMapDictionariesQuery
    {
        Task<IReadOnlyList<MeasurementMapDictionary>> ExecuteAsync();
    }
}