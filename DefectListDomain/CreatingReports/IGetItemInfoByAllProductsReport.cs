using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.CreatingReports
{
    public interface IGetItemInfoByAllProductsReport
    {
        Task<bool> CreateAsync(
            IEnumerable<IBomHeader> bomHeaders,
            IEnumerable<IBomItem> bomItems,
            string createdBy);
    }
}