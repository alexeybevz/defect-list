using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.CreatingReports
{
    public interface IMeasurementMapReport
    {
        Task<bool> CreateAsync(
            MeasurementMap map,
            BomItem bomItem,
            BomHeader bomHeader,
            string createdBy);
    }
}