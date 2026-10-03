using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Dtos;
using DefectListDomain.Models;

namespace DefectListDomain.CreatingReports
{
    public interface IExportCreatedRouteMapsReport
    {
        Task<bool> CreateAsync(
            IBomHeader bomHeader,
            IReadOnlyList<RouteMapDto> items,
            string createdBy);
    }
}