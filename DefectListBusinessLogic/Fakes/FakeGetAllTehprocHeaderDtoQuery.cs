using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefectListDomain.Dtos;
using DefectListDomain.ExternalData;

namespace DefectListBusinessLogic.Fakes
{
    public class FakeGetAllTehprocHeaderDtoQuery : IGetAllTehprocHeaderDtoQuery
    {
        public Task<IEnumerable<TehprocHeaderDto>> Execute()
        {
            var fakeData = new List<TehprocHeaderDto>();
            return Task.FromResult(fakeData.AsEnumerable());
        }

        public Task<TehprocHeaderDto> ExecuteByDetals(string detals)
        {
            var fakeData = new TehprocHeaderDto();
            return Task.FromResult(fakeData);
        }
    }
}