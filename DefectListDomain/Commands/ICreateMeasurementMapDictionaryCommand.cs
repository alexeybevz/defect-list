using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    // Создаёт новый справочник (шапку без строк).
    // Version=1, IsActive=1 проставляются автоматически.
    public interface ICreateMeasurementMapDictionaryCommand
    {
        Task<MeasurementMapDictionary> ExecuteAsync(MeasurementMapDictionary dictionary, IReadOnlyList<RootItem> rootItems, string createdBy);
    }
}