using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Dtos;

namespace DefectListDomain.ExternalData
{
    public interface IGetAllRouteMapDtoQuery
    {
        Task<IEnumerable<RouteMapDto>> ExecuteAsync(IEnumerable<string> routeMaps);
    }
}