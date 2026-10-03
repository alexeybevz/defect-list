using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.ExternalData;

namespace DefectListBusinessLogic.Fakes
{
    public class FakeGetAllDistinctShopEntriesQuery : IGetAllDistinctShopEntriesQuery
    {
        public async Task<Dictionary<int, string>> Execute()
        {
            var fakeData = new Dictionary<int, string>();
            return await Task.FromResult(fakeData);
        }
    }
}