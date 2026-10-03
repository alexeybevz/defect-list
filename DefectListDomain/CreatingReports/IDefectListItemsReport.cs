using System.Threading.Tasks;
using DefectListDomain.Models;
using DefectListDomain.ReportParameters;

namespace DefectListDomain.CreatingReports
{
    public interface IDefectListItemsReport
    {
        Task<bool> CreateAsync(
            IBomHeader bomHeader,
            DefectListItemsRptParm rptParm,
            string createdBy);
    }
}