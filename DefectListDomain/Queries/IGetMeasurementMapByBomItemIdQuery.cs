using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Queries
{
    public interface IGetMeasurementMapByBomItemIdQuery
    {
        Task<MeasurementMap> ExecuteAsync(int bomItemId);
    }
}