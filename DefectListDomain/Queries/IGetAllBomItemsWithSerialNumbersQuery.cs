using System.Collections.Generic;
using System.Threading.Tasks;

namespace DefectListDomain.Queries
{
    public interface IGetAllBomItemsWithSerialNumbersQuery
    {
        Task<Dictionary<string, IEnumerable<int>>> Execute();
    }
}