using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface ICopyMeasurementMapDictionaryCommand
    {
        Task<int> ExecuteAsync(int sourceMeasurementMapDictionaryId, MeasurementMapDictionary targetDictionary, IReadOnlyList<RootItem> rootItems, string createdBy);
    }
}