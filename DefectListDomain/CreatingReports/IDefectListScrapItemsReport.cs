using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.CreatingReports
{
    public interface IDefectListScrapItemsReport
    {
        Task<bool> CreateAsync(
            IBomHeader bomHeader,
            IReadOnlyCollection<BomItem> data,
            string createdBy);
    }
}