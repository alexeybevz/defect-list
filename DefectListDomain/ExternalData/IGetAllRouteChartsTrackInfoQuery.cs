using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.ExternalData
{
    public interface IGetAllRouteChartsTrackInfoQuery
    {
        Task<IEnumerable<RouteChartTrackInfo>> Execute(string order);
    }
}