using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.CreatingReports
{
    public interface IDefectListItemsChangesReport
    {
        Task<bool> CreateAsync(
            IBomHeader bomHeader,
            IReadOnlyCollection<BomItem> bomItems,
            IReadOnlyCollection<BomItemLog> bomItemsLogs,
            string createdBy);
    }
}