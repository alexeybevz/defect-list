using System.Collections.Generic;

namespace DefectListDomain.ExternalData
{
    public interface IGetAllIsExpensiveClassifierQuery
    {
        List<int> Execute();
    }
}