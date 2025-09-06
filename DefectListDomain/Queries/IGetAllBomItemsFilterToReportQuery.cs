using System.Collections.Generic;
using System.Threading.Tasks;

namespace DefectListDomain.Queries
{
    public interface IGetAllBomItemsFilterToReportQuery
    {
        Task<IEnumerable<string>> Execute(int rootItemId, string reportName);
    }
}