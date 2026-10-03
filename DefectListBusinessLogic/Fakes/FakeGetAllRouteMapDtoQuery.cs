using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefectListDomain.Dtos;
using DefectListDomain.ExternalData;

namespace DefectListBusinessLogic.Fakes
{
    public class FakeGetAllRouteMapDtoQuery : IGetAllRouteMapDtoQuery
    {
        public Task<IEnumerable<RouteMapDto>> ExecuteAsync(IEnumerable<string> routeMaps)
        {
            return Task.FromResult(new List<RouteMapDto>().AsEnumerable());
        }
    }
}