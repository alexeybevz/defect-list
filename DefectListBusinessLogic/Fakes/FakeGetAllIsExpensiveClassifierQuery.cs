using System.Collections.Generic;
using DefectListDomain.ExternalData;

namespace DefectListBusinessLogic.Fakes
{
    public class FakeGetAllIsExpensiveClassifierQuery : IGetAllIsExpensiveClassifierQuery
    {
        public List<int> Execute()
        {
            return new List<int>();
        }
    }
}