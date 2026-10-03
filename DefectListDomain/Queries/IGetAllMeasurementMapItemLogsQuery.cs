using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Queries
{
    public interface IGetAllMeasurementMapItemLogsQuery
    {
        Task<IReadOnlyCollection<MeasurementMapItemLog>> ExecuteByMeasurementMapIdAsync(int measurementMapId);
        Task<IReadOnlyCollection<MeasurementMapItemLog>> ExecuteByBomItemIdAsync(int bomItemId);
    }
}