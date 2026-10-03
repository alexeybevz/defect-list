using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.CreatingReports
{
    public interface IChangesFinalDecisionReport
    {
        Task<bool> CreateAsync(
            IReadOnlyCollection<FinalDecisionChanging> data,
            IReadOnlyDictionary<int, string> productsDistinctShopEntries,
            string createdBy);
    }
}