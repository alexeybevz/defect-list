using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefectListDomain.Dtos;
using DefectListDomain.ExternalData;

namespace DefectListBusinessLogic.Fakes
{
    public class FakeGetAllWpDtoQuery : IGetAllWpDtoQuery
    {
        public Task<IEnumerable<WpDto>> ExecuteAsync()
        {
            var fakeData = new List<WpDto>();
            return Task.FromResult(fakeData.AsEnumerable());
        }
    }
}