using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Queries
{
    public interface IGetAllMapsBomItemToRouteChartsQuery
    {
        Task<IEnumerable<MapBomItemToRouteChart>> Execute();
        Task<IEnumerable<MapBomItemToRouteChart>> ExecuteByBomItemId(int id);
    }
}