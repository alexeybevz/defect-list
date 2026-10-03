using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.CreatingReports
{
    public interface IAuxiliaryMaterialsReport
    {
        Task<bool> CreateAsync(
            IBomHeader bomHeader,
            IReadOnlyCollection<BomItem> bomItemsView,
            string createdBy);
    }
}