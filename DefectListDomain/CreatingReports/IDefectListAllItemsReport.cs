using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.CreatingReports
{
    public interface IDefectListAllItemsReport
    {
        Task<bool> CreateAsync(
            IBomHeader bomHeader,
            IEnumerable<BomItem> data,
            string createdBy);
    }
}