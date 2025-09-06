using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefectListDomain.ExternalData;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Fakes
{
    public class FakeGetAllRouteChartsTrackInfoQuery : IGetAllRouteChartsTrackInfoQuery
    {
        public Task<IEnumerable<RouteChartTrackInfo>> Execute(string order)
        {
            var fakeData = new List<RouteChartTrackInfo>();
            return Task.FromResult(fakeData.AsEnumerable());
        }
    }
}