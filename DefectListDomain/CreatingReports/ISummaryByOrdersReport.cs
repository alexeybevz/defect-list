using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Dtos;
using DefectListDomain.Models;

namespace DefectListDomain.CreatingReports
{
    public interface ISummaryByOrdersReport
    {
        Task<bool> CreateAsync(
            IEnumerable<BomHeader> bomHeaders,
            IEnumerable<IBomItem> items,
            IEnumerable<ProductDto> products,
            Dictionary<int, string> productsDistinctShopEntries,
            string createdBy);
    }
}